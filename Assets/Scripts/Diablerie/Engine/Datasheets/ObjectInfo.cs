using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class ObjectInfo
    {
        public string nameStr;
        public string description;
        public int id;
        public string token;
        public int spawnMax;
        [Datasheet.Sequence(length = 8)]
        public bool[] selectable;
        public int trapProb;
        public int sizeX;
        public int sizeY;
        public int nTgtFX;
        public int nTgtFY;
        public int nTgtBX;
        public int nTgtBY;
        [Datasheet.Sequence(length = 8)]
        public int[] frameCount;
        [Datasheet.Sequence(length = 8)]
        public int[] frameDelta;
        [Datasheet.Sequence(length = 8)]
        public bool[] cycleAnim;
        [Datasheet.Sequence(length = 8)]
        public int[] lit;
        [Datasheet.Sequence(length = 8)]
        public bool[] blocksLight;
        [Datasheet.Sequence(length = 8)]
        public bool[] hasCollision;
        public bool isAttackable;
        [Datasheet.Sequence(length = 8)]
        public int[] start;
        public int envEffect;
        public bool isDoor;
        public bool blockVis;
        public int orientation;
        public int trans;
        [Datasheet.Sequence(length = 8)]
        public int[] orderFlag;
        public int preOperate;
        [Datasheet.Sequence(length = 8)]
        public bool[] mode;
        public int yOffset;
        public int xOffset;
        public bool draw;
        public int red;
        public int blue;
        public int green;
        [Datasheet.Sequence(length = 16)]
        public bool[] layersSelectable;
        public int totalPieces;
        public int subClass;
        public int xSpace;
        public int ySpace;
        public int nameOffset;
        public string monsterOk;
        public int operateRange;
        public string shrineFunction;
        public string restore;
        [Datasheet.Sequence(length = 8)]
        public int[] parm;
        public int act;
        public int lockable;
        public int gore;
        public int sync;
        public int flicker;
        public int damage;
        public int beta;
        public int overlay;
        public int collisionSubst;
        public int left;
        public int top;
        public int width;
        public int height;
        public int operateFn;
        public int populateFn;
        public int initFn;
        public int clientFn;
        public int restoreVirgins;
        public int blocksMissile;
        public int drawUnder;
        public int openWarp;
        public int autoMap;

        [System.NonSerialized]
        public float[] frameDuration = new float[8];

        [System.NonSerialized]
        public string name;

        public static List<ObjectInfo> sheet = Datasheet.Load<ObjectInfo>("data/global/excel/objects.txt");
        static Dictionary<string, ObjectInfo> byToken = new Dictionary<string, ObjectInfo>();

        static ObjectInfo()
        {
            foreach(var info in sheet)
            {
                for(int i = 0; i < 8; ++i)
                {
                    info.frameDuration[i] = 256.0f / 25 / info.frameDelta[i];
                }

                info.name = Translation.Find(info.nameStr);

                if (info.token == null)
                    continue;

                if (byToken.ContainsKey(info.token))
                    byToken.Remove(info.token);
                byToken.Add(info.token, info);
            }
        }

        // Warning: token is not a unique identifier
        public static ObjectInfo Find(string token)
        {
            return byToken.GetValueOrDefault(token);
        }
    }
}
