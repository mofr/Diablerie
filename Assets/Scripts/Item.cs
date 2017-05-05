using UnityEngine;

public class Item
{
    public readonly ItemInfo info;
    Sprite _invSprite;

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
    }

    public Sprite invSprite
    {
        get
        {
            if (_invSprite == null)
            {
                var dc6 = DC6.Load(info.invFile);
                _invSprite = dc6.GetSprites(0)[0];
            }
            return _invSprite;
        }
    }
}
