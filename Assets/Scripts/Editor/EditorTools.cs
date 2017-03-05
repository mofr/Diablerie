using System.IO;
using UnityEngine;
using UnityEditor;

public class EditorTools
{
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

        Palette.LoadPalette(1);
        var result = DT1.Import(assetPath);
        int i = 0;
        foreach (var texture in result.textures)
        {
            var pngData = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            var pngPath = assetPath + "." + i + ".png";
            File.WriteAllBytes(pngPath, pngData);
            AssetDatabase.ImportAsset(pngPath);
            ++i;
        }
    }

    [MenuItem("Assets/Convert DT1 to PNG", true)]
    static public bool ConvertDT1ToPNGValidate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("dt1");
    }

    [MenuItem("Assets/Convert DCC to PNG")]
    static public void ConvertDCCToPNG()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        Palette.LoadPalette(1);
        DCC dcc = DCC.Load(assetPath, loadAllDirections: true, ignoreCache: true);
        int i = 0;
        foreach (var texture in dcc.textures)
        {
            var pngData = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            var pngPath = assetPath + "." + i + ".png";
            File.WriteAllBytes(pngPath, pngData);
            AssetDatabase.ImportAsset(pngPath);
            ++i;
        }
    }

    [MenuItem("Assets/Convert DCC to PNG", true)]
    static public bool ConvertDCCToPNGValidate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("dcc");
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

    [MenuItem("Assets/Test serialization")]
    static public void TestSerialization()
    {
        var rb = Obj.Find(1, 2, 2);
        Debug.Log(rb.description);
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
