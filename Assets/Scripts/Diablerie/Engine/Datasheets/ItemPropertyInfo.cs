using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class ItemPropertyInfo
    {
        public static List<ItemPropertyInfo> sheet = Datasheet.Load<ItemPropertyInfo>("data/global/excel/Properties.txt");
        static Dictionary<string, ItemPropertyInfo> map = new Dictionary<string, ItemPropertyInfo>();

        public static ItemPropertyInfo Find(string code)
        {
            return map.GetValueOrDefault(code);
        }

        static ItemPropertyInfo()
        {
            sheet.RemoveAll(row => !row._done);

            foreach (var prop in sheet)
            {
                map.Add(prop.code, prop);

                for(int i = 0; i < prop._blocks.Length; ++i)
                {
                    Block block = prop._blocks[i];
                    if (block.func == 0)
                        break;
                    block.stat = ItemStat.Find(block.statId);
                    prop.blocks.Add(block);
                }
            }
        }

        [System.Serializable]
        [Datasheet.Record]
        public struct Block
        {
            public string set;
            public int value;
            public int func;
            public string statId;

            [System.NonSerialized]
            public ItemStat stat;
        }

        public string code;
        public bool _done;
        [Datasheet.Sequence(length = 7)]
        public Block[] _blocks;
        public string _desc;
        public string _param;
        public string _min;
        public string _max;
        public string _notes;
        public string _eol;

        [System.NonSerialized]
        public List<Block> blocks = new List<Block>();
    }
}
