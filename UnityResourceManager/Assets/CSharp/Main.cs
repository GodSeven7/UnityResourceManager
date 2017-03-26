using UnityEngine;
using System.Collections;
using ResMgr;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        ResourceManager.getInstance().Update();
	}

    void OnGUI()
    {
        //测试代码
        if(GUILayout.Button("Load AssetBundle"))
        {
            LoadTestAsset();
        }
        if(GUILayout.Button("GC"))
        {
            test1 = null;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }


    void LoadTestAsset()
    {
        AssetBundleManager.getInstance().LoadManifest();

        ResourceManager.getInstance().LoadAssetAsync("prefab_AB", "Cube", OnLoadCallBack1);
        ResourceManager.getInstance().LoadAssetAsync("prefab_AB", "Sphere", OnLoadCallBack2);
        ResourceManager.getInstance().LoadAssetAsync("prefab_AB", "Capsule", OnLoadCallBack3);
    }

    void OnLoadCallBack1(RefAsset ra)
    {
        Debug.Log("OnLoadCallBack1");
        ra.GetInstantiateObject();
    }

    UnityEngine.Object test1;
    UnityEngine.Object test2;

    void OnLoadCallBack2(RefAsset ra)
    {
        Debug.Log("OnLoadCallBack2");
        test1 = ra.GetPrefabObject();
        GameObject go = GameObject.Instantiate(test1) as GameObject;
    }

    void OnLoadCallBack3(RefAsset ra)
    {
        Debug.Log("OnLoadCallBack3");
        test2 = ra.GetPrefabObject();
        GameObject go = GameObject.Instantiate(test2) as GameObject;
    }
}
