using System.Collections.Generic;

[System.Serializable]
public class MonStat
{
    [System.Serializable]
    public class TreasureClassInfo
    {
        public string _normal;
        public string _champion;
        public string _unique;
        public string _quest;

        [System.NonSerialized]
        public TreasureClass normal;

        [System.NonSerialized]
        public TreasureClass champion;

        [System.NonSerialized]
        public TreasureClass unique;

        [System.NonSerialized]
        public TreasureClass quest;
    }

    const int DifficultyCount = 3;

    public string id;
    public int hcIndex;
    public string baseId;
    public string nextInClass;
    public string transLvl;
    public string nameStr;
    public string monStatEx;
    public string monProp;
    public string monType;
    public string ai;
    public string descStr;
    public string code;
    public bool enabled;
    public string rangedType;
    public string placeSpawn;
    public string spawn;
    public string spawnX;
    public string spawnY;
    public string spawnMode;
    public string minion1Id;
    public string minion2Id;
    public string SetBoss;
    public string BossXfer;
    public int partyMin = 0;
    public int partyMax = 0;
    public int minGrp;
    public int maxGrp;
    public string sparsePopulate;
    public int speed;
    public int runSpeed;
    public string Rarity;
    [Datasheet.Sequence(length = DifficultyCount)]
    public int[] level;
    public string monSoundId;
    public string uMonSoundId;
    public string threat;
    [Datasheet.Sequence(length = DifficultyCount)]
    public string[] aidel;
    [Datasheet.Sequence(length = DifficultyCount)]
    public string[] aidist;
    [Datasheet.Sequence(length = 8 * DifficultyCount)]
    public string[] aip1;
    public string MissA1;
    public string MissA2;
    public string MissS1;
    public string MissS2;
    public string MissS3;
    public string MissS4;
    public string MissC;
    public string MissSQ;
    public string Align;
    public string isSpawn;
    public string isMelee;
    public bool npc;
    public string interact;
    public string inventory;
    public string inTown;
    public string lUndead;
    public string hUndead;
    public string demon;
    public string flying;
    public string opendoors;
    public string boss;
    public string primeevil;
    public string killable;
    public string switchai;
    public string noAura;
    public string nomultishot;
    public string neverCount;
    public string petIgnore;
    public string deathDmg;
    public string genericSpawn;
    public string zoo;
    public string SendSkills;

    public string Skill1;
    public string Sk1mode;
    public string Sk1lvl;
    [Datasheet.Sequence(length = 3 * 7)]
    public string[] remainingSkillsInfo;

    [Datasheet.Sequence(length = DifficultyCount)]
    public string[] drain;
    [Datasheet.Sequence(length = DifficultyCount)]
    public string[] coldEffect;

    public string ResDm;
    public string ResMa;
    public string ResFi;
    public string ResLi;
    public string ResCo;
    public string ResPo;
    [Datasheet.Sequence(length = 6 * (DifficultyCount - 1))]
    public string[] remainingResInfo;

    public string DamageRegen;
    public string skillDamage;
    public bool noRatio;
    public string NoShldBlock;
    [Datasheet.Sequence(length = DifficultyCount)]
    public string[] toBlock;
    public string Crit;

    public int minHP;
    public int maxHP;
    public string AC;
    public string Exp;
    public string A1MinD;
    public string A1MaxD;
    public string A1TH;
    public string A2MinD;
    public string A2MaxD;
    public string A2TH;
    public string S1MinD;
    public string S1MaxD;
    public string S1TH;
    [Datasheet.Sequence(length = 13 * (DifficultyCount - 1))]
    public string[] repeatedStruct;

    [Datasheet.Sequence(length = 3 * (2 + 4 * DifficultyCount))]
    public string[] elementalDamage;
    [Datasheet.Sequence(length = DifficultyCount)]
    public TreasureClassInfo[] treasureClass;
    public string TCQuestId;
    public string TCQuestCP;
    public string SplEndDeath;
    public string SplGetModeChart;
    public string SplEndGeneric;
    public string SplClientEnd;
    string eol;

    [System.NonSerialized]
    public MonStatsExtended ext;

    [System.NonSerialized]
    public string name;

    [System.NonSerialized]
    public MonStat minion1;

    [System.NonSerialized]
    public MonStat minion2;

    [System.NonSerialized]
    public MonSound sound;

    [System.NonSerialized]
    public MonSound uniqueSound;

    public static List<MonStat> sheet = Datasheet.Load<MonStat>("data/global/excel/monstats.txt");
    static Dictionary<string, MonStat> stats = new Dictionary<string, MonStat>();

    static MonStat()
    {
        foreach(MonStat stat in sheet)
        {
            var key = stat.id.ToLower();
            if (stats.ContainsKey(key))
            {
                stats.Remove(key);
            }
            stats.Add(key, stat);
            stat.ext = MonStatsExtended.Find(stat.monStatEx);
            stat.name = stat.nameStr == null ? null : Translation.Find(stat.nameStr);
            stat.minion1 = stat.minion1Id == null ? null : Find(stat.minion1Id);
            stat.minion2 = stat.minion2Id == null ? null : Find(stat.minion2Id);
            stat.sound = MonSound.Find(stat.monSoundId);
            stat.uniqueSound = MonSound.Find(stat.uMonSoundId);
            foreach(var tcInfo in stat.treasureClass)
            {
                tcInfo.normal = TreasureClass.Find(tcInfo._normal);
                tcInfo.champion = TreasureClass.Find(tcInfo._champion);
                tcInfo.unique = TreasureClass.Find(tcInfo._unique);
                tcInfo.quest = TreasureClass.Find(tcInfo._quest);
            }
        }
    }

    public static MonStat Find(string id)
    {
        if (id == null)
            return null;
        return stats.GetValueOrDefault(id, null);
    }
}
