using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class SpawnPreset
    {
        public int act;
        public int type;
        public int id;
        public string description;
        public int objectId = -1;
        public int monstatId = -1;
        public int direction = 0;
        public string _base;
        public string token;
        public string mode;
        public string weaponClass;
        [Datasheet.Sequence(length = 16)]
        public string[] gear;
        public string colormap;
        public string index;
        public string eol;

        public static List<SpawnPreset> sheet = Datasheet.Load<SpawnPreset>("/obj.txt");
        static Dictionary<long, SpawnPreset> lookup = new Dictionary<long, SpawnPreset>();

        static SpawnPreset()
        {
            foreach (SpawnPreset obj in sheet)
            {
                lookup.Add(Key(obj.act - 1, obj.type, obj.id), obj);
            }
        }

        static long Key(int act, int type, int id)
        {
            long key = act;

            key <<= 2;
            key += type;

            key <<= 32;
            key += id;

            return key;
        }

        public static SpawnPreset Find(int act, int type, int id)
        {
            SpawnPreset obj = null;
            lookup.TryGetValue(Key(act, type, id), out obj);
            return obj;
        }
    }
}
