using UnityEngine;
using System.Collections;

public class Config
{
    static int _loadAsetBundleCount = -1;

    public static int mLoaderAssetBundleCount
    {
        get { return _loadAsetBundleCount; }
        set { _loadAsetBundleCount = value; }
    }

    static int _loadPrefabCount = -1;

    public static int mLoaderPrefabCount
    {
        get { return _loadPrefabCount; }
        set { _loadPrefabCount = value; }
    }
}
