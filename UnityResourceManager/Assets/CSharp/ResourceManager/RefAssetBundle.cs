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

        public RefAsstBundle(string abName)
        {
            _abName = abName;
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
            if (_ab == null)
                AssetBundleLoader.getInstance().PreLoader(this);

            List<RefAsstBundle> rabList = this.GetDependenceRef();
            IEnumerator it = rabList.GetEnumerator();
            while (it.MoveNext())
            {
                RefAsstBundle subRab = (RefAsstBundle)it.Current;
                subRab.PreLoadAssetBundle();
            }
        }

        public void StartLoad()
        {
            if (_ab == null && _abcr == null)
                _abcr = AssetBundle.LoadFromFileAsync(Application.dataPath + "/AssetBundle/" + _abName);
        }

        public bool IsDone()
        {
            if (_ab != null)
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
            _refCount++;

            if (_rabList != null)
            {
                IEnumerator iter = _rabList.GetEnumerator();
                while (iter.MoveNext())
                {
                    ((RefAsstBundle)iter.Current).IncreaseRef();
                }
            }
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
            }

            if (_rabList != null)
            {
                IEnumerator iter = _rabList.GetEnumerator();
                while (iter.MoveNext())
                {
                    ((RefAsstBundle)iter.Current).DecreaseRef();
                }
            }
        }

        public AssetBundleRequest LoadPrefabAynsc(string assetName)
        {
            return _ab.LoadAssetAsync(assetName);
        }
    }
}