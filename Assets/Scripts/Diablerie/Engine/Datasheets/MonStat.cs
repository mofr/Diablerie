using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class MonStat
    {
        [System.Serializable]
        [Datasheet.Record]
        public struct TreasureClassInfo
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

        [System.Serializable]
        [Datasheet.Record]
        public struct Stats
        {
            public int minHP;
            public int maxHP;
            public int armorClass; // AC
            public uint exp;
            public int A1MinDamage; // A1MinD
            public int A1MaxDamage; // A1MaxD
            public int A1ToHit; // A1TH
            public int A2MinDamage;
            public int A2MaxDamage;
            public int A2ToHit;
            public int S1MinDamage;
            public int S1MaxDamage;
            public int S1ToHit;
        }

        const int DifficultyCount = 3;

        public string id;
        public int hcIndex;
        public string baseId;
        public string nextInClass;
        public uint transLvl;
        public string nameStr;
        public string monStatEx;
        public string monProp;
        public string monType;
        public string ai;
        public string descStr;
        public string code;
        public bool enabled;
        public bool rangedType;
        public bool placeSpawn;
        public string spawn;
        public int spawnX;
        public int spawnY;
        public string spawnMode;
        public string minion1Id;
        public string minion2Id;
        public bool SetBoss;
        public bool BossXfer;
        public int partyMin = 0;
        public int partyMax = 0;
        public int minGrp;
        public int maxGrp;
        public uint sparsePopulate;
        public uint speed;
        public uint runSpeed;
        public uint Rarity;
        [Datasheet.Sequence(length = DifficultyCount)]
        public int[] level;
        public string monSoundId;
        public string uMonSoundId;
        public int threat;
        [Datasheet.Sequence(length = DifficultyCount)]
        public int[] aidel;
        [Datasheet.Sequence(length = DifficultyCount)]
        public int[] aidist;
        [Datasheet.Sequence(length = 8 * DifficultyCount)]
        public int[] aip1;
        public string MissA1;
        public string MissA2;
        public string MissS1;
        public string MissS2;
        public string MissS3;
        public string MissS4;
        public string MissC;
        public string MissSQ;
        public int Align; // 0, 1, 2
        public bool isSpawn;
        public bool isMelee;
        public bool npc;
        public bool interact;
        public bool inventory;
        public bool inTown;
        public bool lUndead;
        public bool hUndead;
        public bool demon;
        public bool flying;
        public bool opendoors;
        public bool boss;
        public bool primeevil;
        public bool killable;
        public bool switchai;
        public bool noAura;
        public bool nomultishot;
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

        [Datasheet.Sequence(length = DifficultyCount)]
        public Stats[] stats;

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
        public string eol;

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

        public static List<MonStat> sheet;
        static Dictionary<string, MonStat> monStats;

        public static void Load()
        {
            sheet = Datasheet.Load<MonStat>("data/global/excel/MonStats.txt");
            monStats = new Dictionary<string, MonStat>();
            foreach(MonStat stat in sheet)
            {
                var key = stat.id.ToLower();
                if (monStats.ContainsKey(key))
                {
                    monStats.Remove(key);
                }
                monStats.Add(key, stat);
                stat.ext = MonStatsExtended.Find(stat.monStatEx);
                stat.name = stat.nameStr == null ? null : Translation.Find(stat.nameStr);
                stat.minion1 = stat.minion1Id == null ? null : Find(stat.minion1Id);
                stat.minion2 = stat.minion2Id == null ? null : Find(stat.minion2Id);
                stat.sound = MonSound.Find(stat.monSoundId);
                stat.uniqueSound = MonSound.Find(stat.uMonSoundId);
                for(int i = 0; i < stat.treasureClass.Length; ++i)
                {
                    stat.treasureClass[i].normal = TreasureClass.Find(stat.treasureClass[i]._normal);
                    stat.treasureClass[i].champion = TreasureClass.Find(stat.treasureClass[i]._champion);
                    stat.treasureClass[i].unique = TreasureClass.Find(stat.treasureClass[i]._unique);
                    stat.treasureClass[i].quest = TreasureClass.Find(stat.treasureClass[i]._quest);
                }
            }
        }

        public static MonStat Find(string id)
        {
            if (id == null)
                return null;
            return monStats.GetValueOrDefault(id.ToLower(), null);
        }
    }
}
