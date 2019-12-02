using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class SkillDescription
    {
        public string id;
        public int skillPage;
        public int skillRow;
        public int skillColumn;
        public int listRow;
        public int listPool;
        public int iconIndex;
        public string strName;
        public string strShort;
        public string strLong;
        public string strAlt;
        public string strMana;
        public string descDam;
    
        [Datasheet.Sequence(length = 100)]
        public string[] otherFields;

        public string eol;

        public static List<SkillDescription> sheet;
        static Dictionary<string, SkillDescription> map;

        public static void Load()
        {
            sheet = Datasheet.Load<SkillDescription>("data/global/excel/SkillDesc.txt");
            map = new Dictionary<string, SkillDescription>();
            foreach (var row in sheet)
            {
                map.Add(row.id, row);
            }
        }

        public static SkillDescription Find(string id)
        {
            if (id == null)
                return null;
            var result = map.GetValueOrDefault(id);
            if (result != null)
                return result;

            return null;
        }
    }
}
