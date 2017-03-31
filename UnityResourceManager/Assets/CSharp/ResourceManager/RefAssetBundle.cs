using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResMgr
{
    //记录ab的引用计数,同时负责ab的加载
    public class RefAsstBundle
    {
        string _abName;
        AssetBundle _ab;
        int _refCount = 0;
        AssetBundleCreateRequest _abcr;
        List<RefAsstBundle> _rabList;

        enum Status
        {
            UnLoaded,
            Loading,
            Loaded,
        }
        Status _status;

        public RefAsstBundle(string abName)
        {
            _abName = abName;
            _status = Status.UnLoaded;
        }

        public void InitDependenceRef()
        {
            if (_rabList == null)
            {
                _rabList = new List<RefAsstBundle>();
                string[] arr = AssetBundleManager.getInstance().GetDependence(_abName);
                for (int i = 0; i < arr.Length; i++)
                {
                    string s = arr[i];
                    RefAsstBundle rab = AssetBundleManager.getInstance().TryGetAssetBundle(s);
                    _rabList.Add(rab);

                    rab.InitDependenceRef();
                }
            }
        }

        public List<RefAsstBundle> GetDependenceRef()
        {
            return _rabList;
        }

        public void PreLoadAssetBundle()
        {
            if (_status == Status.UnLoaded)
                AssetBundleLoader.getInstance().PreLoader(this);

            List<RefAsstBundle> rabList = this.GetDependenceRef();
            IEnumerator it = rabList.GetEnumerator();
            while (it.MoveNext())
            {
                RefAsstBundle subRab = (RefAsstBundle)it.Current;
                subRab.PreLoadAssetBundle();
            }
        }

        public void StartLoadAsync()
        {
            if (_status == Status.UnLoaded)
            {
                _abcr = AssetBundle.LoadFromFileAsync(Application.dataPath + "/AssetBundle/" + _abName);
                _status = Status.Loading;
            }
        }

        public bool IsDone()
        {
            if (_status == Status.Loaded)
                return true;

            if (_abcr == null)
            {
                Debug.LogError(string.Format("{0} ab is not loading", _abName));
                return false;
            }

            if (_abcr.isDone && CheckDependenceIsDone())
            {
                //判断依赖项是否都已经ab完成
                _ab = _abcr.assetBundle;
                _abcr = null;
                _status = Status.Loaded;
                return true;
            }

            return false;
        }

        bool CheckDependenceIsDone()
        {
            if (_rabList != null)
            {
                IEnumerator iter = _rabList.GetEnumerator();
                while (iter.MoveNext())
                {
                    bool isDone = ((RefAsstBundle)iter.Current).IsDone();
                    if (!isDone)
                        return false;
                }
            }
            return true;
        }

        public void IncreaseRef()
        {
            if (_rabList != null && _refCount == 0)
            {
                IEnumerator iter = _rabList.GetEnumerator();
                while (iter.MoveNext())
                {
                    ((RefAsstBundle)iter.Current).IncreaseRef();
                }
            }

            _refCount++;
        }

        public void DecreaseRef()
        {
            _refCount--;
            if (_refCount < 0)
            {
                Debug.LogError(string.Format("{0} refCount < 0", _abName));
                return;
            }
            else if (_refCount == 0)
            {
                if (_ab == null)
                {
                    Debug.LogError(string.Format("DecreaseRef {0} ab = null", _abName));
                    return;
                }
                _ab.Unload(true);
                _ab = null;
                Debug.Log(string.Format("Remove AssetBundle {0}", _abName));

                _status = Status.UnLoaded;

                if (_rabList != null)
                {
                    IEnumerator iter = _rabList.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        ((RefAsstBundle)iter.Current).DecreaseRef();
                    }
                }
            }

        }

        public AssetBundleRequest LoadPrefabAsync(string assetName)
        {
            return _ab.LoadAssetAsync(assetName);
        }

        public AssetBundleRequest LoadAllAssetsAsync()
        {
            return _ab.LoadAllAssetsAsync();
        }

        //-----------------------------------同步操作-----------------------------------------------//
        public void StartLoadSync()
        {
            if(_status != Status.Loaded)
            {
                _ab = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundle/" + _abName);
                if(_ab == null)
                {
                    return;
                }

                if(_rabList.Count > 0)
                {
                    IEnumerator it = _rabList.GetEnumerator();
                    while(it.MoveNext())
                    {
                        ((RefAsstBundle)it.Current).StartLoadSync();
                    }
                }
            }

            _status = Status.Loaded;
        }

        public UnityEngine.Object LoadPrefabSync(string assetName)
        {
            if (_ab == null)
            {
                return null;
            }

            return _ab.LoadAsset(assetName);
        }

        public UnityEngine.Object[] LoadAllAssetsSync()
        {
            if (_ab == null)
            {
                return null;
            }

            return _ab.LoadAllAssets();
        }
    }
}