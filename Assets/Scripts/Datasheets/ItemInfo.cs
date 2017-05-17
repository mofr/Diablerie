using System.Collections.Generic;

[System.Serializable]
public class ItemInfo
{
    public static List<ArmorInfo> armorSheet = Datasheet.Load<ArmorInfo>("data/global/excel/armor.txt");
    public static List<WeaponInfo> weaponSheet = Datasheet.Load<WeaponInfo>("data/global/excel/weapons.txt");
    public static List<MiscInfo> miscSheet = Datasheet.Load<MiscInfo>("data/global/excel/misc.txt");
    public static Dictionary<string, ItemInfo>.ValueCollection all;

    protected static Dictionary<string, ItemInfo> byCode = new Dictionary<string, ItemInfo>();

    public static ItemInfo Find(string code)
    {
        return byCode.GetValueOrDefault(code);
    }

    public void GatherTypes(IList<ItemType> result)
    {
        if (type1 != null)
            type1.GatherTypes(result);

        if (type2 != null)
            type2.GatherTypes(result);
    }

    public bool HasType(ItemType type)
    {
        return (type1 != null && type1.Is(type)) || (type2 != null && type2.Is(type));
    }

    static ItemInfo()
    {
        LoadArmorInfo();
        LoadWeaponInfo();
        LoadMiscInfo();
        SetupLinks();
        all = byCode.Values;
    }

    private static void LoadWeaponInfo()
    {
        foreach (var item in weaponSheet)
        {
            if (item._code == null)
                continue;

            item.code = item._code;
            item.cost = item._cost;
            item.gambleCost = item._gambleCost;
            item.flippyFile = item._flippyFile;
            item.invFile = item._invFile;
            item.invWidth = item._invWidth;
            item.invHeight = item._invHeight;
            item.level = item._level;
            item.levelReq = item._levelReq;
            item.weapon = item;
            item.name = Translation.Find(item.nameStr);
            item.type1Code = item._type1;
            item.type2Code = item._type2;
            item.component = item._component;
            item.alternateGfx = item._alternateGfx;
            item.dropSound = SoundInfo.Find(item._dropSound);
            item.dropSoundFrame = item._dropSoundFrame;
            item.useSound = SoundInfo.Find(item._useSound);

            if (!byCode.ContainsKey(item.code))
                byCode.Add(item.code, item);
        }
    }

    private static void LoadArmorInfo()
    {
        foreach (var item in armorSheet)
        {
            if (item._code == null)
                continue;

            item.code = item._code;
            item.cost = item._cost;
            item.gambleCost = item._gambleCost;
            item.flippyFile = item._flippyFile;
            item.invFile = item._invFile;
            item.invWidth = item._invWidth;
            item.invHeight = item._invHeight;
            item.level = item._level;
            item.levelReq = item._levelReq;
            item.armor = item;
            item.name = Translation.Find(item.nameStr);
            item.type1Code = item._type1;
            item.type2Code = item._type2;
            item.component = item._component;
            item.alternateGfx = item._alternateGfx;
            item.dropSound = SoundInfo.Find(item._dropSound);
            item.dropSoundFrame = item._dropSoundFrame;
            item.useSound = SoundInfo.Find(item._useSound);

            if (!byCode.ContainsKey(item.code))
                byCode.Add(item.code, item);
        }
    }

    private static void LoadMiscInfo()
    {
        foreach (var item in miscSheet)
        {
            if (item._code == null)
                continue;

            item.code = item._code;
            item.cost = item._cost;
            item.gambleCost = item._gambleCost;
            item.flippyFile = item._flippyFile;
            item.invFile = item._invFile;
            item.invWidth = item._invWidth;
            item.invHeight = item._invHeight;
            item.level = item._level;
            item.levelReq = item._levelReq;
            item.misc = item;
            item.name = Translation.Find(item.nameStr);
            item.type1Code = item._type1;
            item.type2Code = item._type2;
            item.component = item._component;
            item.alternateGfx = item._alternateGfx;
            item.dropSound = SoundInfo.Find(item._dropSound);
            item.dropSoundFrame = item._dropSoundFrame;
            item.useSound = SoundInfo.Find(item._useSound);

            if (!byCode.ContainsKey(item.code))
                byCode.Add(item.code, item);
        }
    }

    private static void SetupLinks()
    {
        foreach(var item in byCode.Values)
        {
            if (item.type1Code != null)
                item.type1 = ItemType.Find(item.type1Code);

            if (item.type2Code != null)
                item.type2 = ItemType.Find(item.type2Code);

            item.type = item.type1 != null ? item.type1 : item.type2;
            item.flippyFile = @"data\global\items\" + item.flippyFile + ".dc6";
        }
    }

    [System.NonSerialized]
    public string code;

    [System.NonSerialized]
    public string name;

    [System.NonSerialized]
    public int cost;

    [System.NonSerialized]
    public int gambleCost;

    [System.NonSerialized]
    public string flippyFile;

    [System.NonSerialized]
    public string invFile;

    [System.NonSerialized]
    public int invWidth;

    [System.NonSerialized]
    public int invHeight;

    [System.NonSerialized]
    public int level;

    [System.NonSerialized]
    public int levelReq;

    [System.NonSerialized]
    public int component;

    [System.NonSerialized]
    public string alternateGfx;

    [System.NonSerialized]
    public MiscInfo misc;

    [System.NonSerialized]
    public WeaponInfo weapon;

    [System.NonSerialized]
    public ArmorInfo armor;

    [System.NonSerialized]
    public string type1Code;

    [System.NonSerialized]
    public string type2Code;

    [System.NonSerialized]
    public ItemType type;

    [System.NonSerialized]
    public ItemType type1;

    [System.NonSerialized]
    public ItemType type2;

    [System.NonSerialized]
    public SoundInfo dropSound;

    [System.NonSerialized]
    public int dropSoundFrame;

    [System.NonSerialized]
    public SoundInfo useSound;
}
