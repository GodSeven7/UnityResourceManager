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
#if USE_AB
                RefAsstBundle rab = AssetBundleManager.getInstance().TryGetAssetBundle(abName);
                if (rab == null)
                {
                    Debug.LogError(string.Format("Can't create RefAssetBundle , name = {0}", abName));
                    return null;
                }

                ra = new RefAsset(abName, assetName, rab);
#else
                ra = new RefAsset(abName, assetName);
#endif
                _refAssetDic.Add(keyName, ra);
            }
            return ra;
        }

        void ProcAssetWaitQueue()
        {
            int loadCount = _refAssetLoadList.Count;
            int maxCount = ResAgent.mMaxLoadPrefabCount;
            while ((maxCount <= -1 || loadCount < maxCount) && _refAssetWaitQueue.Count > 0)
            {
                RefAsset ra = _refAssetWaitQueue.Dequeue();
                if (_refAssetLoadList.Contains(ra) || _refAssetWaitQueue.Contains(ra))
                    continue;
#if USE_AB
                _refAssetLoadList.Insert(0, ra);
#else
                _refPrefabLoadList.Insert(0, ra);
#endif
                loadCount++;
            }
        }

        void ProcAssetLoadList()
        {
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
        }

        void ProcPrefabLoadList()
        {
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
        }

        void ProcCallBackQueue()
        {
            while (_refCallbackQueue.Count > 0)
            {
                RefAsset ra = _refCallbackQueue.Dequeue();
                ra.RunProc();
            }
        }

        void CheckAssetDic()
        {
            List<string> tmpList = null;
            var iter = _refAssetDic.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Current.Value.TryDestroy())
                {
                    if (tmpList == null)
                        tmpList = new List<string>();

                    tmpList.Add(iter.Current.Key);
                }
            }

            if (tmpList != null)
            {
                int count = tmpList.Count;
                for (int i = 0; i < count; i++)
                {
                    string key = tmpList[i];
                    _refAssetDic.Remove(key);
                }
            }
        }
        public void Update()
        {
            ProcAssetWaitQueue();
            
#if USE_AB
            ProcAssetLoadList();
#endif

            ProcPrefabLoadList();

            ProcCallBackQueue();
            
#if USE_AB
            CheckAssetDic();
#endif
        }
    }
}