using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class MonLvl
    {
        public static List<MonLvl> sheet;
        static Dictionary<int, MonLvl> map;

        public static MonLvl Find(int level)
        {
            return map.GetValueOrDefault(level);
        }

        public static void Load()
        {
            sheet = Datasheet.Load<MonLvl>("data/global/excel/MonLvl.txt");
            map = new Dictionary<int, MonLvl>();
            foreach(var monLvl in sheet)
            {
                map.Add(monLvl.level, monLvl);
            }
        }
    
        public int level;
        [Datasheet.Sequence(length = 6)]
        public int[] armor;
        [Datasheet.Sequence(length = 6)]
        public int[] toHit;
        [Datasheet.Sequence(length = 6)]
        public int[] hp;
        [Datasheet.Sequence(length = 6)]
        public int[] damage;
        [Datasheet.Sequence(length = 6)]
        public int[] experience;
    }
}
