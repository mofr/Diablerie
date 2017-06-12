using System.Collections.Generic;

[System.Serializable]
public class CharStatsInfo
{
    public static List<CharStatsInfo> sheet = Datasheet.Load<CharStatsInfo>("data/global/excel/CharStats.txt");

    public static CharStatsInfo Find(string className)
    {
        return sheet.Find(info => info.className == className);
    }

    public static CharStatsInfo FindByCode(string code)
    {
        return sheet.Find(info => info.code == code);
    }

    [System.Serializable]
    public struct StartingItem
    {
        public string code;
        public string loc;
        public int count;
    }

    static Dictionary<string, string> tokens = new Dictionary<string, string>
    {
        { "Amazon", "AM" },
        { "Sorceress", "SO" },
        { "Necromancer", "NE" },
        { "Paladin", "PA" },
        { "Barbarian", "BA" },
        { "Druid", "DZ" },
        { "Assassin", "AI" },
    };

    static Dictionary<string, string> codes = new Dictionary<string, string>
    {
        { "Amazon", "ama" },
        { "Sorceress", "sor" },
        { "Necromancer", "nec" },
        { "Paladin", "pal" },
        { "Barbarian", "bar" },
        { "Druid", "dru" },
        { "Assassin", "ass" },
    };

    static CharStatsInfo()
    {
        sheet.RemoveAll(row => row.baseWClass == null);
        foreach(var info in sheet)
        {
            info.token = tokens[info.className];
            info.code = codes[info.className];
        }
    }

    public string className;
    public int str;
    public int dex;
    public int energy;
    public int vit;
    public int tot;
    public int stamina;
    public int hpAdd;
    public int percentStr;
    public int percentDex;
    public int percentInt;
    public int percentVit;
    public int manaRegen;
    public int toHitFactor;
    public int walkVelocity;
    public int runVelocity;
    public int runDrain;
    public string comment;
    public int lifePerLevel;
    public int staminaPerLevel;
    public int manaPerLevel;
    public int lifePerVitality;
    public int staminPerVitality;
    public int manaPerMagic;
    public int statPerLevel;
    public int refWalk;
    public int refRun;
    public int refSwing;
    public int refSpell;
    public int refGetHit;
    public int refBow;
    public int blockFactor;
    public string startSkill;
    [Datasheet.Sequence(length = 10)]
    public string[] skills;
    public string strAllSkills;
    public string strSkillTab1;
    public string strSkillTab2;
    public string strSkillTab3;
    public string strClassOnly;
    public string baseWClass;
    [Datasheet.Sequence(length = 10)]
    public StartingItem[] startingItems;

    [System.NonSerialized]
    public string token;

    [System.NonSerialized]
    public string code;
}
