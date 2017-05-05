using UnityEngine;

public class Item
{
    public readonly ItemInfo info;
    Sprite _invSprite;
    int invFileIndex;

    static public Item Create(string code)
    {
        var info = ItemInfo.Find(code);
        if (info == null)
            return null;

        return new Item(info);
    }

    public Item(ItemInfo info)
    {
        this.info = info;
        if (info.type.varInvGfx > 0)
        {
            invFileIndex = Random.Range(0, info.type.varInvGfx);
        }
    }

    public string invFile
    {
        get
        {
            string filename;
            if (info.type.varInvGfx > 0)
                filename = info.type.invGfx[invFileIndex];
            else
                filename = info.invFile;

            filename = @"data\global\items\" + filename + ".dc6";
            return filename;
        }
    }

    public Sprite invSprite
    {
        get
        {
            if (_invSprite == null)
            {
                var dc6 = DC6.Load(invFile);
                _invSprite = dc6.GetSprites(0)[0];
            }
            return _invSprite;
        }
    }
}
