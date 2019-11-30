using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class MiscInfo : ItemInfo
    {
        public enum UseFunction
        {
            None = 0,
            IdentifyItem = 1, 
            TownPortal = 2,
            Potion = 3,
            RejuvPotion = 5,
            TemporaryPotion = 6,
            HoradricCube = 7,
            Elixir = 8,
            StaminaPotion = 9,
        };

        static UseFunction[] useFunctions = new UseFunction[] {
            UseFunction.None,
            UseFunction.IdentifyItem,
            UseFunction.TownPortal,
            UseFunction.Potion,
            UseFunction.None,
            UseFunction.RejuvPotion,
            UseFunction.TemporaryPotion,
            UseFunction.HoradricCube,
            UseFunction.Elixir,
            UseFunction.StaminaPotion,
        };

        public string name1;
        public string name2;
        public string flavorText;
        public bool compactSave;
        public int version;
        public int _level;
        public int _levelReq;
        public int rarity;
        public bool spawnable;
        public int speed;
        public bool noDurability;
        public int _cost;
        public int _gambleCost;
        public string _code;
        public string _alternateGfx;
        public string nameStr;
        public int _component;
        public int _invWidth;
        public int _invHeight;
        public bool _hasInv;
        public int _gemSockets;
        public string _gemApplyType;
        public string _flippyFile;
        public string _invFile;
        public string _uniqueInvFile;
        public string special;
        public string transmogrify;
        public string tMogType;
        public string tMogMin;
        public string tMogMax;
        public bool useable;
        public bool _throwable;
        public string _type1;
        public string _type2;
        public string _dropSound;
        public int _dropSoundFrame;
        public string _useSound;
        public bool _alwaysUnique;
        public string transparent;
        public int transTbl;
        public int lightRadius;
        public bool belt;
        public bool autobelt;
        public bool stackable;
        public int minStack;
        public int maxStack;
        public int spawnStack;
        public int quest;
        public string questDiffCheck;
        public int missileType;
        public string _spellIcon; // all -1
        public int _useFunction; // pSpell
        public string state;
        public string cstate1;
        public string cstate2;
        public int _len;
        public string stat1;
        public int calc1;
        public string stat2;
        public int calc2;
        public string stat3;
        public int calc3;
        [Datasheet.Sequence(length = 106)]
        public string[] skipped2;

        public UseFunction useFunction
        {
            get
            {
                if (_useFunction < 0 || _useFunction >= useFunctions.Length)
                    return UseFunction.None;
                return useFunctions[_useFunction];
            }
        }

        public float duration
        {
            get
            {
                return _len / 25.0f;
            }
        }
    }
}
