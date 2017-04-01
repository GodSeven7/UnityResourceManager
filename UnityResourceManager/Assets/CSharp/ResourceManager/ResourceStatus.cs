using UnityEngine;
using System.Collections;

public struct ResourceStatus 
{
    public string abName;
    public string assetName;
    public int refCount;
    public string proc;

    public ResourceStatus(string name, string asset, int cnt, string status)
    {
        abName = name;
        assetName = asset;
        refCount = cnt;
        proc = status;
    }
}
