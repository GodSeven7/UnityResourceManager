using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResMgr
{
    public class AssetBundleLoader : Singleton<AssetBundleLoader>
    {
        Queue<RefAsset> _refAssetQueue = new Queue<RefAsset>();
        List<RefAsstBundle> _refAssetBundleWaitList = new List<RefAsstBundle>();
        List<RefAsstBundle> _refAssetBundleLoadList = new List<RefAsstBundle>();

        public void PreLoader(RefAsset refAsset)
        {
            _refAssetQueue.Enqueue(refAsset);
        }

        public void Update()
        {
            while(_refAssetQueue.Count > 0)
            {
                RefAsset ra = _refAssetQueue.Dequeue();
                RefAsstBundle rab = ra.mRefAB;
                _refAssetBundleWaitList.Insert(0, rab);
                rab.StartLoad();

                List<RefAsstBundle> rabList = rab.GetDependenceRef();
                IEnumerator it = rabList.GetEnumerator();
                while(it.MoveNext())
                {
                    RefAsstBundle subRab = (RefAsstBundle)it.Current;
                    if (_refAssetBundleWaitList.Contains(subRab))
                        continue;

                    if (_refAssetBundleLoadList.Contains(subRab))
                        continue;

                    _refAssetBundleWaitList.Insert(0, subRab);
                    subRab.StartLoad();
                }
            }

            int count = _refAssetBundleWaitList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                RefAsstBundle rab = _refAssetBundleWaitList[i];
                if (rab.IsDone())
                {
                    _refAssetBundleWaitList.RemoveAt(i);
                }
            }
        }
    }
}
