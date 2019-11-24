using System.Collections.Generic;

[System.Serializable]
[Datasheet.Record]
public class MagicAffix
{
    public static List<MagicAffix> prefixes = Datasheet.Load<MagicAffix>("data/global/excel/MagicPrefix.txt");
    public static List<MagicAffix> suffixes = Datasheet.Load<MagicAffix>("data/global/excel/MagicSuffix.txt");
    public static List<MagicAffix> all = new List<MagicAffix>();

    static MagicAffix()
    {
        foreach (var affix in prefixes)
        {
            affix.prefix = true;
            affix.name = Translation.Find(affix.nameStr);
        }

        foreach (var affix in suffixes)
        {
            affix.name = Translation.Find(affix.nameStr);
        }

        all.AddRange(prefixes);
        all.AddRange(suffixes);
    }

    public static List<MagicAffix> GetSpawnableAffixes(Item item, List<MagicAffix> affixes)
    {
        return affixes.FindAll(affix => 
            affix.spawnable && 
            (affix.rare || item.quality != Item.Quality.Rare)
            );
    }

    [System.Serializable]
    [Datasheet.Record]
    public struct Mod
    {
        public string code;
        public string param;
        public int min;
        public int max;
    }

    public string nameStr;
    public string version;
    public bool spawnable;
    public bool rare;
    public string level;
    public string maxlevel;
    public string levelReq;
    public string classSpecific;
    public string classCode;
    public string classlevelreq;
    public int frequency;
    public int group;
    [Datasheet.Sequence(length = 3)]
    public Mod[] mods;
    public string transform;
    public string transformcolor;
    [Datasheet.Sequence(length = 7)]
    public string[] itemTypes;
    [Datasheet.Sequence(length = 5)]
    public string[] excludeItemTypes;
    public int divide;
    public int multiply;
    public int add;

    [System.NonSerialized]
    public bool prefix = false;

    [System.NonSerialized]
    public string name;
}
