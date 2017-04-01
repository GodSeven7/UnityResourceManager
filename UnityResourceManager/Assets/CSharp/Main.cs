using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
#if USE_AB
        ResAgent.LoadManifest();
#endif
	}
	
	// Update is called once per frame
	void Update () {
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
            test2 = null;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }


    void LoadTestAsset()
    {
        ResAgent.LoadAssetAsync("prefab_ab", "Cube", OnLoadCallBack1);
        RefAsset ra = ResAgent.LoadAsset("prefab_ab", "Sphere");
        ra.GetInstantiateObject();
        //ResourceManager.getInstance().LoadAssetAsync("prefab_AB", "Sphere", OnLoadCallBack2);
        //ResourceManager.getInstance().LoadAssetAsync("prefab_AB", "Capsule", OnLoadCallBack3);
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
        GameObject.Instantiate(test1);
    }

    void OnLoadCallBack3(RefAsset ra)
    {
        Debug.Log("OnLoadCallBack3");
        test2 = ra.GetPrefabObject();
        GameObject.Instantiate(test2);
    }
}
