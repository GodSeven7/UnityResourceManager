using UnityEngine;
using System.Collections;

public class Config
{
    static int _loaderCount = -1;

    public static int mLoaderCount
    {
        get { return _loaderCount; }
        set { _loaderCount = value; }
    }
}
