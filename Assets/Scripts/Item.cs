using System.Text;
using UnityEngine;

public class Item
{
    public enum Quality
    {
        Unique,
        Set,
        Rare,
        Magic,
        HiQuality,
        Normal,
        LowQuality
    }

    public readonly ItemInfo info;
    public Quality quality = Quality.Normal;
    public int level;
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

    void AppendColored(StringBuilder sb, string str, string color)
    {
        sb.Append("<color=#");
        sb.Append(color);
        sb.Append(">");
        sb.Append(str);
        sb.Append("</color>");
    }

    public string GetTitle()
    {
        var sb = new StringBuilder();
        string color = Colors.ItemNormalHex;
        switch (quality)
        {
            case Quality.Unique: color = Colors.ItemUniqueHex; break;
            case Quality.Rare: color = Colors.ItemRareHex; break;
            case Quality.Set: color = Colors.ItemSetHex; break;
            case Quality.Magic: color = Colors.ItemMagicHex; break;
            case Quality.LowQuality: color = Colors.ItemLowQualityHex; break;
        }
        if (quality == Quality.HiQuality)
            AppendColored(sb, Translation.Find("Hiquality") + " ", color);
        else if (quality == Quality.LowQuality)
            AppendColored(sb, Translation.Find("Low Quality") + " ", color);
        AppendColored(sb, info.name, color);
        return sb.ToString();
    }

    public string GetDescription()
    {
        var sb = new StringBuilder();
        sb.Append(GetTitle());

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

        if (quality == Quality.Unique || quality == Quality.Rare || 
            quality == Quality.Set || quality == Quality.Magic)
        {
            AppendColored(sb, "\nUnidentified", Colors.ItemRedHex);
        }

        return sb.ToString();
    }
}
