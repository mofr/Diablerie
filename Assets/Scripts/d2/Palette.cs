using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette
{
    static public Color[] palette;
    static Dictionary<int, Color[]> palettes = new Dictionary<int, Color[]>();

	static public Color[] LoadPalette(int act)
    {
        if (palettes.ContainsKey(act))
        {
            palette = palettes[act];
            return palette;
        }

        palette = new Color[256];
        var stream = new BufferedStream(File.OpenRead("Assets/d2/data/global/palette/ACT" + act + "/Pal.PL2"));
        var reader = new BinaryReader(stream);
        for (int i = 0; i < 256; ++i)
        {
            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();
            reader.ReadByte();

            palette[i] = new Color(r / 255f, g / 255f, b / 255f);
        }
        stream.Close();
        palettes[act] = palette;
        return palette;
    }
}
