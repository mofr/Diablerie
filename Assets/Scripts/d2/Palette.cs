using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palette
{
    static public Color32[] palette;
    static Dictionary<int, Color32[]> palettes = new Dictionary<int, Color32[]>();

	static public Color32[] LoadPalette(int act)
    {
        if (palettes.ContainsKey(act))
        {
            palette = palettes[act];
            return palette;
        }

        palette = new Color32[256];
        using (var stream = new MemoryStream(File.ReadAllBytes(Application.streamingAssetsPath + "/d2/data/global/palette/ACT" + act + "/Pal.PL2")))
        using (var reader = new BinaryReader(stream))
        {
            for (int i = 0; i < 256; ++i)
            {
                byte r = reader.ReadByte();
                byte g = reader.ReadByte();
                byte b = reader.ReadByte();
                reader.ReadByte();

                palette[i] = new Color32(r, g, b, 255);
            }
        }
        palette[0] = new Color(0, 0, 0, 0);
        palettes[act] = palette;
        return palette;
    }
}
