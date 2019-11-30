using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class SetItem
    {
        public static List<SetItem> sheet = Datasheet.Load<SetItem>("data/global/excel/SetItems.txt");

        static SetItem()
        {
            foreach(var item in sheet)
            {
                if (item.itemCode == null)
                    continue;

                item.name = Translation.Find(item.id);
                item.dropSound = SoundInfo.Find(item._dropSound);
                item.dropSoundFrame = item._dropSoundFrame;
                item.useSound = SoundInfo.Find(item._useSound);
                item.itemInfo = ItemInfo.Find(item.itemCode);
                item.set = ItemSet.Find(item.setId);
                item.set.items.Add(item);
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

        public string id;
        public string setId;
        public string itemCode;
        public string _item;
        public int rarity;
        public int level;
        public int levelReq;
        public string chrTransform;
        public string invTransform;
        public string invFile;
        public string flippyFile;
        public string _dropSound;
        public int _dropSoundFrame = -1;
        public string _useSound;
        public string costMult;
        public string costAdd;
        public int addFunc;
        [Datasheet.Sequence(length = 9)]
        public Prop[] props;
        [Datasheet.Sequence(length = 10)]
        public Prop[] additionalProps;
        public string eol;

        [System.NonSerialized]
        public string name;

        [System.NonSerialized]
        public SoundInfo dropSound;

        [System.NonSerialized]
        public int dropSoundFrame;

        [System.NonSerialized]
        public SoundInfo useSound;

        [System.NonSerialized]
        public ItemSet set;

        [System.NonSerialized]
        public ItemInfo itemInfo;
    }
}
