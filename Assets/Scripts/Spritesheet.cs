using UnityEngine;

public abstract class Spritesheet
{
    public abstract Sprite[] GetSprites(int direction);

    public static Spritesheet Load(string filename)
    {
        try
        {
            return DCC.Load(filename + ".dcc");
        }
        catch (System.IO.FileNotFoundException)
        {
            return DC6.Load(filename + ".dc6");
        }
    }
}
