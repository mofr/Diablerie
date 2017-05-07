using System.Text;
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

    public string GetDescription()
    {
        var sb = new StringBuilder();
        sb.Append(info.name);

        if (info.weapon != null)
        {
            if (!info.weapon.twoHanded || info.weapon.oneOrTwoHanded)
                sb.Append("\nOne-Hand Damage: " + info.weapon.minDamage + " to " + info.weapon.maxDamage);
            if (info.weapon.twoHanded)
                sb.Append("\nTwo-Hand Damage: " + info.weapon.twoHandedMinDamage + " to " + info.weapon.twoHandedMaxDamage);
            if (!info.weapon.noDurability)
                sb.Append("\nDurability: " + info.weapon.durability);
            if (info.weapon.reqDex > 0)
                sb.Append("\nRequired Dexterity: " + info.weapon.reqDex);
            if (info.weapon.reqStr > 0)
                sb.Append("\nRequired Strength: " + info.weapon.reqStr);
        }

        if (info.armor != null)
        {
            sb.Append("\nDefense: " + info.armor.minAC);
            if (!info.armor.noDurability)
                sb.Append("\nDurability: " + info.armor.durability);
            if (info.armor.reqStr > 0)
                sb.Append("\nRequired Strength: " + info.armor.reqStr);
        }

        if (info.levelReq > 0)
            sb.Append("\nRequired Level: " + info.levelReq);
        return sb.ToString();
    }
}
