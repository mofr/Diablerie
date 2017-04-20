using System.Collections.Generic;
using UnityEngine;

public class Palette
{
    static public Color32[] palette;
    static public Dictionary<int, Color32[]> palettes = new Dictionary<int, Color32[]>();

    static public Color32[] LoadPalette(int act)
    {
        if (palettes.ContainsKey(act))
        {
            palette = palettes[act];
            return palette;
        }

        palette = new Color32[256];
        var bytes = Mpq.ReadAllBytes(@"data\global\palette\ACT" + (act + 1) + @"\Pal.PL2");
        for (int i = 0; i < 256; ++i)
        {
            int offset = i * 4;
            byte r = bytes[offset];
            byte g = bytes[offset + 1];
            byte b = bytes[offset + 2];
            palette[i] = new Color32(r, g, b, 255);
        }
        palette[0] = new Color(0, 0, 0, 0);
        palettes[act] = palette;
        return palette;
    }
}
