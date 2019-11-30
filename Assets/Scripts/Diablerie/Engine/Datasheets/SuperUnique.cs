using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class SuperUnique
    {
        public static List<SuperUnique> sheet = Datasheet.Load<SuperUnique>("data/global/excel/SuperUniques.txt");
        static Dictionary<string, SuperUnique> map = new Dictionary<string, SuperUnique>();

        public static SuperUnique Find(string key)
        {
            return map.GetValueOrDefault(key);
        }

        static SuperUnique()
        {
            foreach (var row in sheet)
            {
                row.monStat = MonStat.Find(row.monStatId);
                row.name = Translation.Find(row.nameStr);
                map.Add(row.superUnique, row);
            }
        }

        public string superUnique;
        public string nameStr;
        public string monStatId;
        public int hcIdx;
        public string monSound;
        public int mod1;
        public int mod2;
        public int mod3;
        public int minGrp;
        public int maxGrp;
        public int eClass;
        public bool autoPos;
        public int stacks;
        public bool replacable;
        [Datasheet.Sequence(length = 3)]
        public int[] uTrans;
        [Datasheet.Sequence(length = 3)]
        public string[] treasureClass;
        public string eol;

        [System.NonSerialized]
        public MonStat monStat;

        [System.NonSerialized]
        public string name;
    }
}
