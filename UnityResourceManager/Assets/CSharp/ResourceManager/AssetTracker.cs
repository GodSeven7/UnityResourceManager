using UnityEngine;
using System.Collections;

namespace ResMgr
{
    public class AssetTracker : MonoBehaviour
    {
        string _abName = "";
        string _assetName = "";
        public void RecordRef(string abName, string assetName)
        {
            _abName = abName;
            _assetName = assetName;
        }

        void OnDestroy()
        {
            if (string.IsNullOrEmpty(_abName))
                return;

            RefAsset ra = LoaderManager.getInstance().TryGetRefAsset(_abName, _assetName);
            ra.DecreaseRef();
        }
    }
}
