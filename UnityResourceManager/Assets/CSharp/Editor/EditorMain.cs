using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorMain {

	[MenuItem("TestAssetBundle/BuildAssetBundle")]
    public static void BuildAssetBundle()
	{
        Debug.Log("start BuildAssetBundle");
        string output = Application.dataPath + "/AssetBundle";
        if (!Directory.Exists(output))
            Directory.CreateDirectory(output);

        BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
	}
}
