using UnityEngine;
using System;
using System.Collections;
using ResMgr;


//提供给用户访问的资源对象
public class RefAsset
{
    public enum LoadProc
    {
        NONE,               //空闲状态
        START,              //标记开始工作
        PRELOAD_AB,         //准备加载AssetBundle
        LOADING_AB,         //正在加载AssetBundle
        FINISH_AB,          //完成加载AssetBundle
        PRELOAD_PREFAB,     //准备加载Prefab
        LOADING_PREFAB,     //正在加载Prefab
        FINISH_PREFAB,      //完成加载Prefab
    }

    string _abName;
    string _assetName;
    LoadProc _proc;
    Action<RefAsset> _callback;
    RefAsstBundle _rab;
    AssetBundleRequest _abr;
    UnityEngine.Object _prefabObject;
    WeakReference _wr;
    int _refCount = -1;
    float _overTime;


    public string mAbName
    {
        get { return _abName; }
    }

    public LoadProc mProc
    {
        get { return _proc; }
    }
    public RefAsstBundle mRefAB
    {
        get { return _rab; }
    }

    public RefAsset(string abName, string assetName, RefAsstBundle rab)
    {
        _abName = abName;
        _assetName = assetName;
        _rab = rab;
        _proc = LoadProc.NONE;

        if (_rab != null)
        {
            _rab.IncreaseRef();
        }

        _overTime = UnityEngine.Time.realtimeSinceStartup;
    }

    public void SetCallBackFunc(Action<RefAsset> funcCallBack)
    {
        _callback += funcCallBack;
    }

    public void ResetProc()
    {
        _proc = LoadProc.START;
    }

    public void RunProc()
    {
        //按照流水线方式一步步往后执行
        switch (_proc)
        {
            case LoadProc.START:
                _proc = LoadProc.LOADING_AB;
                _rab.PreLoadAssetBundle();
                break;

            case LoadProc.LOADING_AB:
                if (_rab != null && _rab.IsDone())
                {
                    _proc = LoadProc.FINISH_AB;
                }
                break;

            case LoadProc.FINISH_AB:
                _proc = LoadProc.LOADING_PREFAB;
                _abr = _rab.LoadPrefabAynsc(_assetName);
                break;

            case LoadProc.LOADING_PREFAB:
                if (_abr != null && _abr.isDone)
                {
                    _proc = LoadProc.FINISH_PREFAB;
                    _prefabObject = _abr.asset;
                    _wr = new WeakReference(_abr.asset);
                    _abr = null;
                }
                break;

            case LoadProc.FINISH_PREFAB:
                _proc = LoadProc.NONE;
                DoCallBack();
                break;
        }
    }

    void DoCallBack()
    {
        if (_callback != null)
        {
            _callback(this);
            _callback = null;
        }

        _overTime = UnityEngine.Time.realtimeSinceStartup;
    }

    public UnityEngine.Object GetPrefabObject()
    {
        if (_wr == null)
        {
            Debug.LogError(string.Format("Can't found weakreference, abName = {0}, assetName = {1}", _abName, _assetName));
            return null;
        }

        _prefabObject = null;
        return _wr.Target as UnityEngine.Object;
    }

    public UnityEngine.Object GetInstantiateObject()
    {
        if (_wr.Target != null)
        {
            GameObject go = GameObject.Instantiate(_wr.Target as UnityEngine.Object) as GameObject;
            AssetTracker at = go.AddComponent<AssetTracker>();
            at.RecordRef(_abName, _assetName);
            IncreaseRef();
            return go;
        }

        Debug.LogError(string.Format("Can't found weakreference, abName = {0}, assetName = {1}", _abName, _assetName));
        return null;
    }

    void IncreaseRef()
    {
        if (_refCount == -1)
            _refCount = 0;

        _refCount++;
    }

    public void DecreaseRef()
    {
        _refCount--;
        if (_refCount < 0)
        {
            Debug.LogError(string.Format("{0} {1} refCount < 0", _abName, _assetName));
            return;
        }
    }

    public bool TryDestroy()
    {
        if (_refCount > 0)
            return false;

        if ((_refCount == 0) ||
            (_wr != null && _wr.Target == null) ||
            (_refCount == -1 && _prefabObject != null && UnityEngine.Time.realtimeSinceStartup - _overTime > ResAgent.mAutoGCTime))
        {
            _wr = null;
            _prefabObject = null;
            _rab.DecreaseRef();
            return true;
        }

        return false;
    }
}