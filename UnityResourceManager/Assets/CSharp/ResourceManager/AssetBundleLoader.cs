using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResMgr
{
    public class AssetBundleLoader : Singleton<AssetBundleLoader>
    {
        List<RefAsstBundle> _refAssetBundleWaitList = new List<RefAsstBundle>();
        List<RefAsstBundle> _refAssetBundleLoadList = new List<RefAsstBundle>();

        public void PreLoader(RefAsstBundle refAssetBundle)
        {
            if (_refAssetBundleWaitList.Contains(refAssetBundle))
                return;

            if (_refAssetBundleLoadList.Contains(refAssetBundle))
                return;

            _refAssetBundleWaitList.Add(refAssetBundle);
        }

        public void Update()
        {
            int loadCount = _refAssetBundleLoadList.Count;
            while (loadCount < 5 && _refAssetBundleWaitList.Count > 0)
            {
                RefAsstBundle rab = _refAssetBundleWaitList[0];
                _refAssetBundleWaitList.RemoveAt(0);

                rab.StartLoad();
                _refAssetBundleLoadList.Add(rab);

                loadCount++;
            }

            int count = _refAssetBundleLoadList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                RefAsstBundle rab = _refAssetBundleLoadList[i];
                if (rab.IsDone())
                {
                    _refAssetBundleLoadList.RemoveAt(i);
                }
            }
        }
    }
}
