using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ResMgr
{
    //管理AssetBundle
    public class AssetBundleManager : Singleton<AssetBundleManager>
    {
        AssetBundleManifest _manifestFile = null;
        List<string> _abNameList = null;
        Dictionary<string, RefAsstBundle> _abDic = new Dictionary<string, RefAsstBundle>();

        public void LoadManifest()
        {
            if (_manifestFile == null)
            {
                AssetBundle ab = AssetBundle.LoadFromFile(Application.dataPath + "/AssetBundle/AssetBundle");
                if (ab != null)
                {
                    _manifestFile = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                    string[] abNameArr = _manifestFile.GetAllAssetBundles();
                    _abNameList = new List<string>(abNameArr);
                }
            }
        }

        public bool IsExsitAssetFile(string name)
        {
            return _abNameList.Contains(name);
        }

        public string[] GetDependence(string abName)
        {
            if (_manifestFile != null)
            {
                return _manifestFile.GetDirectDependencies(abName);
            }

            return null;
        }

        public RefAsstBundle TryGetAssetBundle(string abName)
        {
            RefAsstBundle rab;
            _abDic.TryGetValue(abName, out rab);
            if (rab == null)
            {
                rab = new RefAsstBundle(abName);
                rab.InitDependenceRef();
                _abDic.Add(abName, rab);
            }
            return rab;
        }
    }

}