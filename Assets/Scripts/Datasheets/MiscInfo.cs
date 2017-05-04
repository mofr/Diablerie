[System.Serializable]
public class MiscInfo : ItemInfo
{
    public string name1;
    public string name2;
    public string flavorText;
    public bool compactSave;
    public int version;
    public int _level;
    public int _levelReq;
    public int rarity;
    public bool spawnable;
    public int speed;
    public bool noDurability;
    public int _cost;
    public int _gambleCost;
    public string _code;
    public string _alternateGfx;
    public string nameStr;
    public int _component;
    [Datasheet.Sequence(length = 5)]
    public string[] skipped;
    public string _flippyFile;
    public string _invFile;
    public string _uniqueInvFile;
    public string special;
    public string transmogrify;
    public string tMogType;
    public string tMogMin;
    public string tMogMax;
    public bool _usable;
    public bool _throwable;
    public string _type1;
    public string _type2;
    [Datasheet.Sequence(length = 134)]
    public string[] skipped2;
}
