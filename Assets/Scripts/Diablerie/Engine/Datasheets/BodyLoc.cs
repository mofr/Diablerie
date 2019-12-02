using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class BodyLoc
    {
        public static List<BodyLoc> sheet;

        public static int GetIndex(string code)
        {
            for (int i = 0; i < sheet.Count; ++i)
            {
                if (sheet[i].code == code)
                    return i;
            }
            return -1;
        }

        public static void Load()
        {
            sheet = Datasheet.Load<BodyLoc>();
            sheet.RemoveAll(loc => loc.code == null);
        }
    
        public string name;
        public string code;
    }
}
