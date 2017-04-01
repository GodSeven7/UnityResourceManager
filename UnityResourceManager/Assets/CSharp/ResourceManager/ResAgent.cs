using UnityEngine;
using System;
using System.Collections;
using ResMgr;

public class ResAgent : MonoBehaviour {
    public static int mMaxLoadAssetBundleCount = -1;

    public static int mMaxLoadPrefabCount = -1;

    public static int mAutoGCTime = 1 * 60;

    void Update()
    {
        LoaderManager.getInstance().Update();
#if USE_AB
        AssetBundleLoader.getInstance().Update();
#endif
	}

#if USE_AB
    public static void LoadManifest()
    {
        AssetBundleManager.getInstance().LoadManifest();
    }
#endif

    public static RefAsset LoadAsset(string abName, string assetName)
    {
#if USE_AB
        if (!AssetBundleManager.getInstance().IsExsitAssetFile(abName))
        {
            Debug.LogError("Can't find assetbundle by name = " + abName);
            return null;
        }
#endif

        //创建或找到RefAsset 放入待处理队列中
        RefAsset ra = LoaderManager.getInstance().TryGetRefAsset(abName, assetName);
        if (ra == null)
        {
            Debug.LogError(string.Format("Can't Create RefAsset abName = {0}, assetName = {1}", abName, assetName));
            return null;
        }

        ra.StartLoadPrefab();

        return ra;
    }

    public static RefAsset LoadAllAsset(string abName)
    {
        if (!AssetBundleManager.getInstance().IsExsitAssetFile(abName))
        {
            Debug.LogError("Can't find assetbundle by name = " + abName);
            return null;
        }

        //创建或找到RefAsset 放入待处理队列中
        RefAsset ra = LoaderManager.getInstance().TryGetRefAsset(abName, "");
        if (ra == null)
        {
            Debug.LogError(string.Format("Can't Create RefAsset abName = {0}", abName));
            return null;
        }

        ra.StartLoadAllAssets();

        return ra;
    }

    public static RefAsset LoadAssetAsync(string abName, string assetName, Action<RefAsset> funcCallBack = null)
    {
#if USE_AB
        if (!AssetBundleManager.getInstance().IsExsitAssetFile(abName))
        {
            Debug.LogError("Can't find assetbundle by name = " + abName);
            return null;
        }
#endif
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
        else if(ra.mProc == RefAsset.LoadProc.END)
        {
            ra.RunProc();
        }

        return ra;
    }

    public static RefAsset LoadAllAssetAsync(string abName, Action<RefAsset> funcCallBack = null)
    {
#if USE_AB
        if (!AssetBundleManager.getInstance().IsExsitAssetFile(abName))
        {
            Debug.LogError("Can't find assetbundle by name = " + abName);
            return null;
        }
#endif

        //创建或找到RefAsset 放入待处理队列中
        RefAsset ra = LoaderManager.getInstance().TryGetRefAsset(abName, "");
        if (ra == null)
        {
            Debug.LogError(string.Format("Can't Create RefAsset abName = {0}", abName));
            return null;
        }
        ra.SetCallBackFunc(funcCallBack);

        if (ra.mProc == RefAsset.LoadProc.NONE)
        {
            ra.ResetProc();
            LoaderManager.getInstance().PreLoader(ra);
        }
        else if (ra.mProc == RefAsset.LoadProc.END)
        {
            ra.RunProc();
        }

        return ra;
    }
}
