using System.Collections.Generic;
using System.IO;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using Diablerie.Engine.World;
using UnityEditor;
using UnityEngine;

namespace Editor
{
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
            Color32[] palette;
            if (lowerPath.Contains("act2"))
                palette = Palette.GetPalette(PaletteType.Act2);
            else if (lowerPath.Contains("act3"))
                palette = Palette.GetPalette(PaletteType.Act3);
            else if (lowerPath.Contains("act4"))
                palette = Palette.GetPalette(PaletteType.Act4);
            else if (lowerPath.Contains("act5"))
                palette = Palette.GetPalette(PaletteType.Act5);
            else
                palette = Palette.GetPalette(PaletteType.Act1);
            var dt1 = DT1.Load(assetPath, palette, mpq: false);
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

            var palette = Palette.GetPalette(PaletteType.Act1);
            DCC dcc = DCC.Load(assetPath, palette, loadAllDirections: true, mpq: false);
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

            var palette = Palette.GetPalette(PaletteType.Act1);
            DC6 dc6 = DC6.Load(assetPath, palette, loadAllDirections: true, mpq: false);
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
            var palette = Palette.GetPalette(PaletteType.Act1);

            int textureSize = 1024;
            if (name.Contains("font16") || name.Contains("font24") || name.Contains("font30"))
                textureSize = 512;

            var dc6 = DC6.Load(assetPath, palette, mpq: false, textureSize: textureSize, loadAllDirections: true);
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
                //int glyphHeight = int.Parse(metrics[i * 2].Trim()); // font16 - all 10, should be used as a line height calculations (like advance for horizontal axis)
                int glyphWidth = int.Parse(metrics[i * 2 + 1].Trim());
                var frame = frames[i];
                characterInfo[i].index = i;
                characterInfo[i].advance = glyphWidth;
                characterInfo[i].minX = 0;
                characterInfo[i].maxX = glyphWidth;
                characterInfo[i].minY = 0;
                characterInfo[i].maxY = frame.height; // doesn't seem to change anything
                characterInfo[i].glyphWidth = glyphWidth;
                characterInfo[i].glyphHeight = frame.height;

                var uv = new Rect(
                    frame.textureX / (float)textureSize,
                    (textureSize - frame.textureY - frame.height) / (float)textureSize,
                    glyphWidth / (float)textureSize,
                    frame.height / (float)textureSize);
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
            string filename = @"data\global\palette\ACT1\Pal.PL2";

            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                using (StormLib.MpqFileStream stream = Mpq.fs.OpenFile(filename))
                {
                    Debug.Log("File size " + stream.Length + " bytes");
                    stream.ReadAllBytes();
                }
                Debug.Log("StormLib fs " + sw.ElapsedMilliseconds + " ms");
            }
        }
    
        [MenuItem("Assets/Generate Datasheet Parsers")]
        static public void GenerateDatasheetLoaders()
        {
            var generator = new DatasheetLoaderGenerator();
            var recordTypes = new List<System.Type>();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var recordAttributes = type.GetCustomAttributes(typeof(Datasheet.Record), true);
                    if (recordAttributes.Length > 0)
                    {
                        recordTypes.Add(type);
                    }
                }
            }
            foreach (var recordType in recordTypes)
            {
                Debug.Log("Generating loader for " + recordType);
                var filename = generator.GenerateToDirectory(recordType, "Assets/Scripts/Generated/DatasheetLoaders/");
                AssetDatabase.ImportAsset(filename);
            }
        }
    }
}
