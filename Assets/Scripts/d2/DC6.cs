using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

class DC6
{
    static public void CreateFontFromDC6(string filename)
    {
        Palette.LoadPalette(1);

        var stream = File.OpenRead(filename);
        var reader = new BinaryReader(stream);

        int dc6_ver1 = reader.ReadInt32();
        var dc6_ver2 = reader.ReadInt32();
        var dc6_ver3 = reader.ReadInt32();
        reader.ReadInt32();
        var dc6_dir = reader.ReadInt32();
        var dc6_fpd = reader.ReadInt32();
        var dc6_fptr = stream.Position;
        if ((dc6_ver1 != 6) || (dc6_ver2 != 1) || (dc6_ver3 != 0))
        {
            Debug.LogWarning("Unknown dc6 version " + dc6_ver1 + " " + dc6_ver2 + " " + dc6_ver3);
            return;
        }

        const int textureSize = 512;
        var texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
        var pixels = new Color32[textureSize * textureSize];
        var packer = new TexturePacker(textureSize, textureSize);
        byte[] data = new byte[1024];
        var characterInfo = new CharacterInfo[dc6_fpd];

        int dir = 0;
        for (int i = 0; i < dc6_fpd; i++)
        {
            stream.Seek(dc6_fptr + (dir * dc6_fpd + i) * 4, SeekOrigin.Begin);
            long offset = reader.ReadInt32();
            stream.Seek(offset, SeekOrigin.Begin);

            reader.ReadInt32();
            int f_w = reader.ReadInt32();
            int f_h = reader.ReadInt32();
            int f_offx = reader.ReadInt32();
            int f_offy = reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            int f_len = reader.ReadInt32();
            if (data.Length < f_len)
                data = new byte[f_len];
            reader.Read(data, 0, f_len);

            var pack = packer.put(f_w, f_h);
            drawFrame(data, f_len, pixels, textureSize, pack.x, pack.y + f_h);

            characterInfo[i].index = i;
            characterInfo[i].advance = f_w;

            characterInfo[i].minX = 0;
            characterInfo[i].maxX = f_w;
            characterInfo[i].minY = -f_h;
            characterInfo[i].maxY = 0;

            characterInfo[i].uv = new Rect(pack.x / (float)textureSize, (textureSize - (pack.y + f_h)) / (float)textureSize,
                f_w / (float)textureSize, f_h / (float)textureSize);
        }

        stream.Close();

        var name = Path.GetFileNameWithoutExtension(filename);
        var filepath = Path.GetDirectoryName(filename) + "/" + name;

        texture.SetPixels32(pixels);
        texture.Apply();
        var pngData = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);
        var texturePath = filepath + ".png";
        File.WriteAllBytes(texturePath, pngData);

        var fontPath = filepath + ".fontsettings";
        AssetDatabase.CreateAsset(new Font(name), fontPath);
        var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
        font.characterInfo = characterInfo;
        EditorUtility.SetDirty(font);

        AssetDatabase.Refresh();
    }

    static void drawFrame(byte[] data, int size, Color32[] pixels, int textureSize, int x0, int y0)
    {
        int dst = textureSize * textureSize - y0 * textureSize - textureSize;
        int ptr = 0;
        int i2, x = x0, y = y0, c, c2;

        for (int i = 0; i < size; i++)
        {
            c = data[ptr];
            ++ptr;

            if (c == 0x80)
            {
                x = x0;
                y--;
                dst += textureSize;
            }
            else if ((c & 0x80) != 0)
                x += c & 0x7F;
            else
            {
                for (i2 = 0; i2 < c; i2++)
                {
                    c2 = data[ptr];
                    ++ptr;
                    i++;
                    pixels[dst + x] = Palette.palette[c2];
                    x++;
                }
            }
        }
    }
}
