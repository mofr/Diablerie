using System.Collections.Generic;
using UnityEngine;

public abstract class Spritesheet
{
    static Dictionary<string, Spritesheet> cache = new Dictionary<string, Spritesheet>();

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
            spritesheet = DCC.Load(filename + ".dcc");
        }
        catch (System.IO.FileNotFoundException)
        {
            spritesheet = DC6.Load(filename + ".dc6");
        }

        cache.Add(lowerFilename, spritesheet);
        return spritesheet;
    }
}
