using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissileInfo
{
    [System.Serializable]
    public struct Param
    {
        public int value;
        public string description;
    }

    public string missile;
    public int id = -1;
    public int clientDoFunc;
    public string clientHitFunc; // string because of values like "*16"
    public int serverDoFunc;
    public string serverHitFunc; // string because of values like "*16"
    public int serverDmgFunc;

    public string serverCalc1;
    public string serverCalc1Description;
    [Datasheet.Sequence(length = 5)]
    public Param[] parameters;

    public string clientCalc1;
    public string clientCalc1Description;
    [Datasheet.Sequence(length = 5)]
    public Param[] clientParameters;

    public string serverHitCalc1;
    public string serverHitCalc1Description;
    [Datasheet.Sequence(length = 3)]
    public Param[] serverHitParameters;

    public string clientHitCalc1;
    public string clientHitCalc1Description;
    [Datasheet.Sequence(length = 3)]
    public Param[] clientHitParameters;

    public string damageCalc1;
    public string damageCalc1Description;
    [Datasheet.Sequence(length = 2)]
    public Param[] damageCalcParameters;

    public int velocity;
    public int maxVelocity;
    public int velocityPerLevel;
    public int accel;
    public int range;
    public int levRange;
    public int light;
    public int flicker;
    public int red;
    public int green;
    public int blue;
    public int initSteps;
    public int activate;
    public int loopAnim;
    public string celFile;
    public int animRate;
    public int animLen;
    public int animSpeed;
    public int randStart;
    public bool subLoop;
    public int subStart;
    public int subStop;
    public int collideType;
    public bool collideKill;
    public bool collideFriend;
    public bool lastCollide;
    public bool collision;
    public bool clientCol;
    public bool clientSend;
    public bool nextHit;
    public int nextDelay;
    public int xOffset;
    public int yOffset;
    public int zOffset;
    public int size;
    public bool srcTown;
    public int clientSrcTown;
    public bool canDestroy;
    public bool toHit;
    public bool alwaysExplode;
    public int explosion; // should be bool
    public bool town;
    public bool noUniqueMod;
    public int noMultiShot; // should be bool
    public string holy;
    public bool canSlow;
    public bool returnFire;
    public bool getHit;
    public bool softHit;
    public int knockBack;
    public int trans;
    public bool useQuantity;
    public bool pierce;
    public string specialSetup;
    public bool missileSkill;
    public string skill;
    public int resultFlags;
    public int hitFlags;
    public int hitShift;
    public bool applyMastery;
    public int srcDamage;
    public int half2HSrc;
    public int srcMissDamage;

    // todo move damage fields to separate structure and share it with SkillInfo
    public int minDamage;
    [Datasheet.Sequence(length = 5)]
    public int[] minDamagePerLevel;
    public int maxDamage;
    [Datasheet.Sequence(length = 5)]
    public int[] maxDamagePerLevel;
    public string damageSymPerCalc;
    public string eType;
    public int eMin;
    [Datasheet.Sequence(length = 5)]
    public int[] minEDamagePerLevel;
    public int eMax;
    [Datasheet.Sequence(length = 5)]
    public int[] maxEDamagePerLevel;
    public string eDamageSymPerCalc;

    public int eLen;
    [Datasheet.Sequence(length = 3)]
    public int[] eLenPerLevel;
    public int hitClass;
    public int numDirections;
    public bool localBlood;
    public int damageRate;
    public string travelSoundId;
    public string hitSoundId;
    public string progSoundId;
    public string progOverlayId;
    public string explosionMissileId;
    [Datasheet.Sequence(length = 3)]
    public string[] subMissileId;
    [Datasheet.Sequence(length = 4)]
    public string[] hitSubMissileId;
    [Datasheet.Sequence(length = 3)]
    public string[] clientSubMissileId;
    [Datasheet.Sequence(length = 4)]
    public string[] clientHitSubMissileId;
    public string eol;

    [System.NonSerialized]
    public string spritesheetFilename;

    [System.NonSerialized]
    public Material material;

    [System.NonSerialized]
    public float lifeTime;

    [System.NonSerialized]
    public float fps;

    [System.NonSerialized]
    public MissileInfo explosionMissile;

    public static List<MissileInfo> sheet = Datasheet.Load<MissileInfo>("data/global/excel/Missiles.txt");
    static Dictionary<string, MissileInfo> map = new Dictionary<string, MissileInfo>();

    static MissileInfo()
    {
        foreach (var row in sheet)
        {
            if (row.id == -1)
                continue;

            row.spritesheetFilename = @"data\global\missiles\" + row.celFile;
            row.material = row.trans == 0 ? Materials.normal : Materials.softAdditive;
            row.lifeTime = row.range / 25.0f;
            row.explosionMissile = Find(row.explosionMissileId);
            row.fps = row.animSpeed * 1.5f;
            map.Add(row.missile, row);
        }
    }

    public static MissileInfo Find(string id)
    {
        if (id == null)
            return null;
        return map.GetValueOrDefault(id);
    }
}
