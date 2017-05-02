[System.Serializable]
public class ArmorInfo : ItemInfo
{
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
    public int _level;
    public int _levelReq;
    public int _cost;
    public int _gambleCost;
    public string _code;
    public string nameStr;
    [Datasheet.Sequence(length = 14)]
    public string[] skipped;
    public string _flippyFile;
    public string _invFile;
    public string _uniqueInvFile;
    public string _setInvFile;
    public string rArm;
    public string lArm;
    public string torso;
    public string legs;
    public string rSPad;
    public string lSPad;
    public bool usable;
    public bool throwable;
    public bool stackable;
    public int minStack;
    public int maxStack;
    public string _type1;
    public string _type2;
    [Datasheet.Sequence(length = 114)]
    public string[] skipped2;
}
