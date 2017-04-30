using System.Collections.Generic;

[System.Serializable]
public class ArmorInfo
{
    public static List<ArmorInfo> sheet = Datasheet.Load<ArmorInfo>("data/global/excel/armor.txt");
    static Dictionary<string, ArmorInfo> byCode = new Dictionary<string, ArmorInfo>();

    public static ArmorInfo Find(string code)
    {
        return byCode.GetValueOrDefault(code);
    }

    static ArmorInfo()
    {
        foreach (ArmorInfo item in sheet)
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
    public string version;
    public string compactSave;
    public int rarity;
    public bool spawnable;
    public int minAC;
    public int maxAC;
    public int absorbs;
    public int speed;
    public int reqStr;
    public int block;
    public int durability;
    public string noDurability;
    public int level;
    public int levelReq;
    public int cost;
    public int gambleCost;
    public string code;
    public string nameStr;
    [Datasheet.Sequence(length = 14)]
    public string[] skipped;
    public string flippyFile;
    public string invFile;
    [Datasheet.Sequence(length = 129)]
    public string[] skipped2;

    [System.NonSerialized]
    public string name;
}
