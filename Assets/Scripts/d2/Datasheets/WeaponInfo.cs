using System.Collections.Generic;

[System.Serializable]
public class WeaponInfo
{
    public static List<WeaponInfo> sheet = Datasheet.Load<WeaponInfo>("data/global/excel/weapons.txt");
    static Dictionary<string, WeaponInfo> byCode = new Dictionary<string, WeaponInfo>();

    public static WeaponInfo Find(string code)
    {
        return byCode.GetValueOrDefault(code);
    }

    static WeaponInfo()
    {
        foreach (WeaponInfo item in sheet)
        {
            if (item.code == null)
                continue;
            if (byCode.ContainsKey(item.code))
                continue;
            item.name = Translation.Find(item.nameStr);
            byCode.Add(item.code, item);
        }
    }

    public string _name;
    public string type;
    public string type2;
    public string code;
    public string alternateGfx;
    public string nameStr;
    public int version;
    public int compactSave;
    public int rarity;
    public bool spawnable;
    public int minDamage;
    public int maxDamage;
    [Datasheet.Sequence(length = 35)]
    public string[] skipped;
    public string flippyFile;
    [Datasheet.Sequence(length = 118)]
    public string[] skipped2;

    [System.NonSerialized]
    public string name;
}
