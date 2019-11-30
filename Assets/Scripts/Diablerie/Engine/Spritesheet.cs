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

        public static Spritesheet Load(string filename)
        {
            string lowerFilename = filename.ToLower();
            if (cache.ContainsKey(lowerFilename))
            {
                return cache[lowerFilename];
            }

            Spritesheet spritesheet = null;
            try
            {
                var dcc = DCC.Load(filename + ".dcc");
                spritesheet = dcc;
                spritesheet.directionCount = dcc.directionCount;
            }
            catch (System.IO.FileNotFoundException)
            {
                var dc6 = DC6.Load(filename + ".dc6");
                spritesheet = dc6;
                spritesheet.directionCount = dc6.directionCount;
            }

            cache.Add(lowerFilename, spritesheet);
            return spritesheet;
        }
    }
}
