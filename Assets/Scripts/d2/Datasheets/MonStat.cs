using System.Collections.Generic;

[System.Serializable]
public class MonStat
{
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
    public string[] level = new string[DifficultyCount];
    public string MonSound;
    public string UMonSound;
    public string threat;
    public string[] aidel = new string[DifficultyCount];
    public string[] aidist = new string[DifficultyCount];
    public string[] aip1 = new string[8 * DifficultyCount];
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
    public string[] remainingSkillsInfo = new string[3 * 7];

    public string[] drain = new string[DifficultyCount];
    public string[] coldEffect = new string[DifficultyCount];

    public string ResDm;
    public string ResMa;
    public string ResFi;
    public string ResLi;
    public string ResCo;
    public string ResPo;
    public string[] remainingResInfo = new string[6 * (DifficultyCount - 1)];

    public string DamageRegen;
    public string skillDamage;
    public string noRatio;
    public string NoShldBlock;
    public string[] toBlock = new string[DifficultyCount];
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
    public string[] repeatedStruct = new string[13 * (DifficultyCount - 1)];

    public string[] elementalDamage = new string[3 * (2 + 4 * DifficultyCount)];
    public string[] treasureClass = new string[4 * 3];
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

    public static Datasheet<MonStat> sheet = Datasheet<MonStat>.Load("data/global/excel/monstats.txt");
    static Dictionary<string, MonStat> stats = new Dictionary<string, MonStat>();

    static MonStat()
    {
        foreach(MonStat stat in sheet.rows)
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
        }
    }

    public static MonStat Find(string id)
    {
        if (id == null)
            return null;
        return stats.GetValueOrDefault(id, null);
    }
}
