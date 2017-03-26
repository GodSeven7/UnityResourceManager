using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResMgr
{
    public class LoaderManager : Singleton<LoaderManager>
    {
        Dictionary<string, RefAsset> _refAssetDic = new Dictionary<string, RefAsset>();
        Queue<RefAsset> _refAssetWaitQueue = new Queue<RefAsset>();
        List<RefAsset> _refAssetLoadList = new List<RefAsset>();
        List<RefAsset> _refPrefabLoadList = new List<RefAsset>();
        Queue<RefAsset> _refCallbackQueue = new Queue<RefAsset>();

        public void PreLoader(RefAsset refAsset)
        {
            _refAssetWaitQueue.Enqueue(refAsset);
        }
        
        public RefAsset TryGetRefAsset(string abName, string assetName)
        {
            RefAsset ra;
            string keyName = abName + assetName;
            _refAssetDic.TryGetValue(keyName, out ra);
            if(ra == null)
            {
                RefAsstBundle rab = AssetBundleManager.getInstance().TryGetAssetBundle(abName);
                if (rab == null)
                {
                    Debug.LogError(string.Format("Can't create RefAssetBundle , name = {0}", abName));
                    return null;
                }

                ra = new RefAsset(abName, assetName, rab);
                _refAssetDic.Add(keyName, ra);
            }
            return ra;
        }

        public void Update()
        {
            int loadCount = _refAssetLoadList.Count;
            while (loadCount < 5 && _refAssetWaitQueue.Count > 0)
            {
                RefAsset ra = _refAssetWaitQueue.Dequeue();
                if (_refAssetLoadList.Contains(ra) || _refAssetWaitQueue.Contains(ra))
                    continue;

                _refAssetLoadList.Insert(0, ra);
                loadCount++;
            }

            //加载AssetBundle
            int abCount = _refAssetLoadList.Count;
            for (int i = abCount - 1; i >= 0; i--)
            {
                RefAsset ra = _refAssetLoadList[i];
                ra.RunProc();
                if (ra.mProc == RefAsset.LoadProc.FINISH_AB)
                {
                    _refAssetLoadList.RemoveAt(i);
                    _refPrefabLoadList.Insert(0, ra);
                }
            }

            //加载Prefab
            int prefabCount = _refPrefabLoadList.Count;
            for (int i = prefabCount - 1; i >= 0; i--)
            {
                RefAsset ra = _refPrefabLoadList[i];
                ra.RunProc();
                if (ra.mProc == RefAsset.LoadProc.FINISH_PREFAB)
                {
                    _refPrefabLoadList.RemoveAt(i);
                    _refCallbackQueue.Enqueue(ra);
                }
            }

            //加载完成回调
            while(_refCallbackQueue.Count > 0)
            {
                RefAsset ra = _refCallbackQueue.Dequeue();
                ra.RunProc();
            }

            //检查所有的refAsset引用, 删除不再使用的refAsset
            List<string> tmpList = null;
            var iter = _refAssetDic.GetEnumerator();
            while(iter.MoveNext())
            {
                if(iter.Current.Value.TryDestroy())
                {
                    if(tmpList == null)
                        tmpList = new List<string>();

                    tmpList.Add(iter.Current.Key);
                }
            }

            if(tmpList != null)
            {
                int count = tmpList.Count;
                for(int i = 0; i < count; i++)
                {
                    string key = tmpList[i];
                    _refAssetDic.Remove(key);
                }
            }
        }
    }
}