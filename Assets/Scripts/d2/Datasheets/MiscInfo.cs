using System.Collections.Generic;

[System.Serializable]
public class MiscInfo
{
    public static List<MiscInfo> sheet = Datasheet.Load<MiscInfo>("data/global/excel/misc.txt");
    static Dictionary<string, MiscInfo> byCode = new Dictionary<string, MiscInfo>();

    public static MiscInfo Find(string code)
    {
        return byCode.GetValueOrDefault(code);
    }

    static MiscInfo()
    {
        foreach (MiscInfo item in sheet)
        {
            if (item.code == null)
                continue;
            if (byCode.ContainsKey(item.code))
                continue;
            item.name = Translation.Find(item.nameStr);
            byCode.Add(item.code, item);
        }
    }

    public string name1;
    public string name2;
    public string flavorText;
    public bool compactSave;
    public int version;
    public int level;
    public int levelReq;
    public int rarity;
    public bool spawnable;
    public int speed;
    public bool noDurability;
    public int cost;
    public int gambleCost;
    public string code;
    public string alternateGfx;
    public string nameStr;
    [Datasheet.Sequence(length = 6)]
    public string[] skipped;
    public string flippyFile;
    public string invFile;
    [Datasheet.Sequence(length = 144)]
    public string[] skipped2;

    [System.NonSerialized]
    public string name;
}
