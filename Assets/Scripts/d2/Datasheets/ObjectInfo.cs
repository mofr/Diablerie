using System.Collections.Generic;

[System.Serializable]
public class ObjectInfo
{
    public string nameStr;
    public string description;
    public int id;
    public string token;
    public int spawnMax;
    public bool[] selectable = new bool[8];
    public int trapProb;
    public int sizeX;
    public int sizeY;
    public int nTgtFX;
    public int nTgtFY;
    public int nTgtBX;
    public int nTgtBY;
    public int[] frameCount = new int[8];
    public int[] frameDelta = new int[8];
    public bool[] cycleAnim = new bool[8];
    public int[] lit = new int[8];
    public bool[] blocksLight = new bool[8];
    public bool[] hasCollision = new bool[8];
    public int isAttackable;
    public int[] start = new int[8];
    public int envEffect;
    public bool isDoor;
    public bool blocksVis;
    public int orientation;
    public int trans;
    public int[] orderFlag = new int[8];
    public int preOperate;
    public bool[] mode = new bool[8];
    public int yOffset;
    public int xOffset;
    public bool draw;
    public int red;
    public int blue;
    public int green;
    public bool[] layersSelectable = new bool[16];
    public int totalPieces;
    public int subClass;
    public int xSpace;
    public int ySpace;
    public int nameOffset;
    public string monsterOk;
    public int operateRange;
    public string shrineFunction;
    public string restore;
    public int[] parm = new int[8];
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

    public static Datasheet<ObjectInfo> sheet = Datasheet<ObjectInfo>.Load("data/global/excel/objects.txt");
    static Dictionary<string, ObjectInfo> byToken = new Dictionary<string, ObjectInfo>();

    static ObjectInfo()
    {
        foreach(var info in sheet.rows)
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
