using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class ItemSet
    {
        public static List<ItemSet> sheet;

        public static ItemSet Find(string id)
        {
            return sheet.Find(set => set.id == id);
        }

        public static void Load()
        {
            sheet = Datasheet.Load<ItemSet>("data/global/excel/Sets.txt");
            foreach(var set in sheet)
            {
                set.name = Translation.Find(set.nameStr);
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

            public Item.Property GetItemProperty()
            {
                var prop = new Item.Property();
                prop.info = ItemPropertyInfo.Find(this.prop);
                prop.param = param;
                prop.value = max;
                prop.min = min;
                prop.max = max;
                return prop;
            }
        }

        public string id;
        public string nameStr;
        public string version;
        public string level;
        [Datasheet.Sequence(length = 8)]
        public Prop[] props;
        [Datasheet.Sequence(length = 8)]
        public Prop[] fullProps;
        public string eol;

        [System.NonSerialized]
        public string name;

        [System.NonSerialized]
        public List<SetItem> items = new List<SetItem>();
    }
}
