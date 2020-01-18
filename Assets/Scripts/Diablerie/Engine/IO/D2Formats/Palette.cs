using System;
using System.Collections.Generic;
using UnityEngine;

namespace Diablerie.Engine.IO.D2Formats
{
    public static class Palette
    {
        private static readonly Dictionary<PaletteType, Color32[]> Palettes = new Dictionary<PaletteType, Color32[]>();
        private static readonly Dictionary<PaletteType, string> PaletteMappings = new Dictionary<PaletteType, string> {
            { PaletteType.Act1, @"data\global\palette\ACT1\Pal.PL2" },
            { PaletteType.Act2, @"data\global\palette\ACT2\Pal.PL2" },
            { PaletteType.Act3, @"data\global\palette\ACT3\Pal.PL2" },
            { PaletteType.Act4, @"data\global\palette\ACT4\Pal.PL2" },
            { PaletteType.Act5, @"data\global\palette\ACT5\Pal.PL2" },
            { PaletteType.EndGame, @"data\global\palette\EndGame\Pal.PL2" },
            { PaletteType.EndGame2, @"data\global\palette\EndGame2\Pal.PL2" },
            { PaletteType.Fechar, @"data\global\palette\fechar\Pal.PL2" },
            { PaletteType.Loading, @"data\global\palette\loading\Pal.PL2" },
            { PaletteType.Menu0, @"data\global\palette\Menu0\Pal.PL2" },
            { PaletteType.Menu1, @"data\global\palette\menu1\Pal.PL2" },
            { PaletteType.Menu2, @"data\global\palette\menu2\Pal.PL2" },
            { PaletteType.Menu3, @"data\global\palette\menu3\Pal.PL2" },
            { PaletteType.Menu4, @"data\global\palette\menu4\Pal.PL2" },
            { PaletteType.Sky, @"data\global\palette\Sky\Pal.PL2" },
            { PaletteType.Trademark, @"data\global\palette\Trademark\Pal.PL2" },
        };

        public static Color32[] GetPalette(int act)
        {
            try
            {
                var paletteType = (PaletteType) act;
                return GetPalette(paletteType);
            }
            catch
            {
                return GetPalette(PaletteType.Act1);
            }
        }
        
        public static Color32[] GetPalette(PaletteType paletteType)
        {
            return Palettes.ContainsKey(paletteType) ? Palettes[paletteType] : LoadPalette(paletteType);
        }
        
        private static Color32[] LoadPalette(PaletteType paletteType)
        {
            var palette = new Color32[256];
            var palettePath = PaletteMappings[paletteType];
            var bytes = Mpq.ReadAllBytes(palettePath);
            for (var i = 0; i < 256; ++i)
            {
                var offset = i * 4;
                var r = bytes[offset];
                var g = bytes[offset + 1];
                var b = bytes[offset + 2];
                palette[i] = new Color32(r, g, b, 255);
            }
            palette[0] = new Color(0, 0, 0, 0);
            Palettes[paletteType] = palette;
            return palette;
        }
    }
}
