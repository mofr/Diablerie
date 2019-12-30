using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class UniqueItem
    {
        public static List<UniqueItem> sheet;

        public static void Load()
        {
            List<UniqueItem> loadedItems = Datasheet.Load<UniqueItem>("data/global/excel/UniqueItems.txt");
            sheet = new List<UniqueItem>(loadedItems.Count);
            foreach (var item in loadedItems)
            {
                if (item.nameStr != null)
                    sheet.Add(item);
            }
            foreach(var unique in sheet)
            {
                unique.name = Translation.Find(unique.nameStr);
                unique.dropSound = SoundInfo.Find(unique._dropSound);
                unique.dropSoundFrame = unique._dropSoundFrame;
                unique.useSound = SoundInfo.Find(unique._useSound);
            }
        }

        [System.Serializable]
        [Datasheet.Record]
        public struct Prop
        {
            public string prop;
            public string param;
            public int min;
            public int max;
        }

        public string nameStr;
        public string version;
        public string enabled;
        public string ladder;
        public int rarity;
        public bool noLimit;
        public int level;
        public int levelReq;
        public string code;
        public string type;
        public string uber;
        public string carry1;
        public string costMult;
        public string costAdd;
        public string chrTransform;
        public string invTransform;
        public string flippyFile;
        public string invFile;
        public string _dropSound;
        public int _dropSoundFrame = -1;
        public string _useSound;
        [Datasheet.Sequence(length = 12)]
        public Prop[] props;
        public string eol;

        [System.NonSerialized]
        public string name;

        [System.NonSerialized]
        public SoundInfo dropSound;

        [System.NonSerialized]
        public int dropSoundFrame;

        [System.NonSerialized]
        public SoundInfo useSound;
    }
}
