using UnityEngine;
using System;
using System.Collections;
using ResMgr;

public class ResAgent : MonoBehaviour {
    public static bool mIsAssetBundle = true;

    public static int mMaxLoadAssetBundleCount = -1;

    public static int mMaxLoadPrefabCount = -1;

    public static int mAutoGCTime = 1 * 60;

    void Update()
    {
        LoaderManager.getInstance().Update();
        AssetBundleLoader.getInstance().Update();
	}

    public static void LoadManifest()
    {
        AssetBundleManager.getInstance().LoadManifest();
    }

    public static RefAsset LoadAssetAsync(string abName, string assetName, Action<RefAsset> funcCallBack = null)
    {
        abName = abName.ToLower();
        assetName = assetName.ToLower();
        if (!AssetBundleManager.getInstance().IsExsitAssetFile(abName))
        {
            Debug.LogError("Can't find assetbundle by name = " + abName);
            return null;
        }

        //创建或找到RefAsset 放入待处理队列中
        RefAsset ra = LoaderManager.getInstance().TryGetRefAsset(abName, assetName);
        if (ra == null)
        {
            Debug.LogError(string.Format("Can't Create RefAsset abName = {0}, assetName = {1}", abName, assetName));
            return null;
        }
        ra.SetCallBackFunc(funcCallBack);

        if (ra.mProc == RefAsset.LoadProc.NONE)
        {
            ra.ResetProc();
            LoaderManager.getInstance().PreLoader(ra);
        }

        return ra;
    }
}
