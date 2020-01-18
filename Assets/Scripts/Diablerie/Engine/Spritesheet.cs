using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Engine
{
    public abstract class Spritesheet
    {
        static Dictionary<string, Spritesheet> cache = new Dictionary<string, Spritesheet>();

        public int directionCount;
        public abstract Sprite[] GetSprites(int direction);

        public static Spritesheet Load(string filename, PaletteType paletteType = PaletteType.Act1)
        {
            string lowerFilename = filename.ToLower();
            if (cache.ContainsKey(lowerFilename))
            {
                return cache[lowerFilename];
            }
            
            var palette = Palette.GetPalette(paletteType);

            Spritesheet spritesheet = null;
            try
            {
                var dcc = DCC.Load(filename + ".dcc", palette);
                spritesheet = dcc;
                spritesheet.directionCount = dcc.directionCount;
            }
            catch (System.IO.FileNotFoundException)
            {
                try
                {
                    var dc6 = DC6.Load(filename + ".dc6", palette);
                    spritesheet = dc6;
                    spritesheet.directionCount = dc6.directionCount;
                }
                catch (System.IO.FileNotFoundException)
                {
                    Debug.LogWarning(filename + " not found in MPQ");
                }
            }

            cache.Add(lowerFilename, spritesheet);
            return spritesheet;
        }
    }
}
