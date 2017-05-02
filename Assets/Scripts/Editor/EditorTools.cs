using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class EditorTools
{
    [MenuItem("Assets/Load DS1")]
    static public void LoadDS1()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!Application.isPlaying)
        {
            DT1.ResetCache();
            DS1.ResetCache();
        }
        var ds1 = DS1.Load(assetPath, mpq: false);
        var level = new LevelBuilder(ds1);
        level.Instantiate(new Vector2i(0, 0));
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

        if (!Application.isPlaying)
        {
            DT1.ResetCache();
        }

        var lowerPath = assetPath.ToLower();
        if (lowerPath.Contains("act2"))
            Palette.LoadPalette(1);
        else if (lowerPath.Contains("act3"))
            Palette.LoadPalette(2);
        else if (lowerPath.Contains("act4"))
            Palette.LoadPalette(3);
        else if (lowerPath.Contains("act5"))
            Palette.LoadPalette(4);
        else
            Palette.LoadPalette(0);
        var dt1 = DT1.Load(assetPath, mpq: false);
        int i = 0;
        foreach (var texture in dt1.textures)
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

        Palette.LoadPalette(0);
        DCC dcc = DCC.Load(assetPath, loadAllDirections: true, mpq: false);
        SaveTextures(assetPath, dcc.textures);
    }

    [MenuItem("Assets/Convert DCC to PNG", true)]
    static public bool ConvertDCCToPNGValidate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("dcc");
    }

    [MenuItem("Assets/Convert DC6 to PNG")]
    static public void ConvertDC6ToPNG()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        Palette.LoadPalette(0);
        DC6 dc6 = DC6.Load(assetPath, loadAllDirections: true, mpq: false);
        SaveTextures(assetPath, dc6.textures);
    }

    static private void SaveTextures(string baseName, IList<Texture2D> textures)
    {
        int i = 0;
        foreach (var texture in textures)
        {
            var pngData = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            var pngPath = baseName + "." + i + ".png";
            File.WriteAllBytes(pngPath, pngData);
            AssetDatabase.ImportAsset(pngPath);
            ++i;
        }
    }

    [MenuItem("Assets/Convert DC6 to PNG", true)]
    static public bool ConvertDC6ToPNGValidate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("dc6");
    }

    [MenuItem("Assets/Create font from DC6")]
    static public void CreateFontFromDC6()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var name = Path.GetFileNameWithoutExtension(assetPath);

        int textureSize = 1024;
        if (name.Contains("font16") || name.Contains("font24") || name.Contains("font30"))
            textureSize = 512;

        var dc6 = DC6.Load(assetPath, mpq: false, textureSize: textureSize, loadAllDirections: true);
        if (dc6.textures.Count != 1)
        {
            Debug.LogError("Font not fit into a single texture");
            return;
        }

        var metrics = File.ReadAllText(Path.GetDirectoryName(assetPath) + "/" + name + ".txt").Split(',');
        var frames = dc6.directions[0].frames;

        var characterInfo = new CharacterInfo[dc6.framesPerDirection];
        for (int i = 0; i < frames.Length; i++)
        {
            int glyphHeight = int.Parse(metrics[i * 2].Trim());
            int glyphWidth = int.Parse(metrics[i * 2 + 1].Trim());
            var frame = frames[i];
            characterInfo[i].index = i;
            characterInfo[i].advance = glyphWidth;
            characterInfo[i].minX = 0;
            characterInfo[i].maxX = glyphWidth;
            characterInfo[i].minY = 0;
            characterInfo[i].maxY = frame.height;
            characterInfo[i].glyphWidth = glyphWidth;
            characterInfo[i].glyphHeight = glyphHeight;

            var uv = new Rect(
                frame.textureX / (float)textureSize,
                (textureSize - frame.textureY - frame.height) / (float)textureSize,
                glyphWidth / (float)textureSize,
                glyphHeight / (float)textureSize);
            characterInfo[i].uvBottomLeft = new Vector2(uv.xMin, uv.yMin);
            characterInfo[i].uvBottomRight = new Vector2(uv.xMax, uv.yMin);
            characterInfo[i].uvTopLeft = new Vector2(uv.xMin, uv.yMax);
            characterInfo[i].uvTopRight = new Vector2(uv.xMax, uv.yMax);
        }

        
        var filepath = "Assets/Fonts/" + name;

        var texture = dc6.textures[0];
        var pngData = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);
        var texturePath = filepath + ".png";
        File.WriteAllBytes(texturePath, pngData);
        AssetDatabase.ImportAsset(texturePath);

        var fontPath = filepath + ".fontsettings";

        var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        if (font)
        {
            font.characterInfo = characterInfo;
            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
        }
        else
        {
            font = new Font(name);
            font.characterInfo = characterInfo;
            AssetDatabase.CreateAsset(font, fontPath);
            AssetDatabase.ImportAsset(fontPath);
        }

        var serializedFont = File.ReadAllBytes(fontPath);
        File.WriteAllText(fontPath, "");
        AssetDatabase.ImportAsset(fontPath);
        File.WriteAllBytes(fontPath, serializedFont);
    }

    [MenuItem("Assets/Create font from DC6", true)]
    static public bool CreateFontFromDC6Validate()
    {
        var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        return assetPath.ToLower().EndsWith("dc6");
    }

    [MenuItem("Assets/Test Action")]
    static public void TestAction()
    {
        Debug.Log(ItemInfo.Find("gth").name);
        Debug.Log(ItemInfo.Find("gsc").name);
        Debug.Log(ItemInfo.Find("gld").name);
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
