using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class MonStatsExtended
    {
        public string id;
        public int height;
        public int OverlayHeight;
        public int pixHeight;
        public int sizeX;
        public int sizeY;
        public int spawnCol;
        public int meleeRng;
        public string baseWeaponClass;
        public string HitClass;
        [Datasheet.Sequence(length = 16)]
        public string[] gearVariantsStr;
        [Datasheet.Sequence(length = 16)]
        public bool[] hasLayer;
        public int totalPieces;
        [Datasheet.Sequence(length = 16)]
        public bool[] hasMode;
        [Datasheet.Sequence(length = 16)]
        public int[] directionCount;
        public bool a1Moving;
        public bool a2Moving;
        public bool scMoving;
        public bool s1Moving;
        public bool s2Moving;
        public bool s3Moving;
        public bool s4Moving;
        public bool noGfxHitTest;
        public int htTop;
        public int htLeft;
        public int htWidth;
        public int htHeight;
        public int restore;
        public int automapCel;
        public int noMap;
        public int noOvly;
        public int isSel;
        public int alSel;
        public int noSel;
        public int shiftSel;
        public int corpseSel;
        public int isAtt;
        public int revive;
        public int critter;
        public int small;
        public int large;
        public int soft;
        public int inert;
        public int objCol;
        public int deadCol;
        public int unflatDead;
        public int shadow;
        public int noUniqueShift;
        public int compositeDeath;
        public int localBlood;
        public int bleed;
        public int light;
        public int lightR;
        public int lightG;
        public int lightB;
        [Datasheet.Sequence(length = 3)]
        public int[] utrans;
        public int heart;
        public int bodyPart;
        public int infernoLen;
        public int infernoAnim;
        public int infernoRollback;
        public string resurrectMode;
        public string resurrectSkill;
        public string eol;

        [System.NonSerialized]
        public string[][] gearVariants = new string[16][];

        public static List<MonStatsExtended> sheet = Datasheet.Load<MonStatsExtended>("data/global/excel/MonStats2.txt");
        static Dictionary<string, MonStatsExtended> stats = new Dictionary<string, MonStatsExtended>();

        static MonStatsExtended()
        {
            foreach (var stat in sheet)
            {
                for(int i = 0; i < stat.gearVariantsStr.Length; ++i)
                {
                    if (stat.gearVariantsStr[i] == null)
                    {
                        continue;
                    }
                    var gearVariants = stat.gearVariantsStr[i].Replace("nil", "").Replace("\"", "").Split(',');
                    stat.gearVariants[i] = gearVariants;
                }
                if (stats.ContainsKey(stat.id))
                {
                    stats.Remove(stat.id);
                }
                stats.Add(stat.id.ToLower(), stat);
            }
        }

        public static MonStatsExtended Find(string id)
        {
            return stats.GetValueOrDefault(id, null);
        }
    }
}
