using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class MonPreset
    {
        const int ActCount = 5;

        public int act;
        public string place;

        public static List<MonPreset> sheet = Datasheet.Load<MonPreset>("data/global/excel/MonPreset.txt");
        static List<MonPreset>[] presets = new List<MonPreset>[ActCount + 1];

        static MonPreset()
        {
            for(int act = 0; act < presets.Length; ++act)
            {
                presets[act] = new List<MonPreset>();
            }

            for(int i = 0; i < sheet.Count; ++i)
            {
                MonPreset preset = sheet[i];
                presets[preset.act].Add(preset);
            }
        }

        public static string Find(int act, int id)
        {
            var actPresets = presets[act];
            if (id < actPresets.Count)
            {
                return actPresets[id].place;
            }
            return null;
        }
    }
}
