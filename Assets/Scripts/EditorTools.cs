using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class EditorTools {
    [MenuItem("Assets/Create/Iso Animation")]
    static public void CreateIsoAnimation()
    {
        ScriptableObjectUtility.CreateAsset<IsoAnimation>();
    }

    [MenuItem("Assets/Load DS1")]
    static public void LoadDS1()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        DS1.Import(assetPath);
    }

    [MenuItem("Assets/Load DS1", true)]
    static public bool LoadDS1Validate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("ds1");
    }

    [MenuItem("Assets/Convert DT1 to PNG")]
    static public void ConvertDT1ToPNG()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        DT1.ConvertToPng(assetPath);
    }

    [MenuItem("Assets/Convert DT1 to PNG", true)]
    static public bool ConvertDT1ToPNGValidate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("dt1");
    }

    [MenuItem("Assets/Reset DT1 cache")]
    static public void ResetDT1()
    {
        DT1.ResetCache();
    }

    [MenuItem("Assets/Create font from DC6")]
    static public void CreateFontFromDC6()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        DC6.CreateFontFromDC6(assetPath);
    }

    [MenuItem("Assets/Create font from DC6", true)]
    static public bool CreateFontFromDC6Validate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("dc6");
    }
}

public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static T CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New" + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        return asset;
    }
}

public class PostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".dt1"))
            {
                //DT1.Import(assetPath);
            }
            else if (assetPath.EndsWith(".ds1"))
            {
                //DS1.Import(assetPath);
            }
        }
    }
}
