using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;


public struct Datasheet<T> where T : new()
{
    public List<T> rows;

    static object CastValue(string value, System.Type type, object defaultValue)
    {
        if (value == "" || value == "xxx")
            return defaultValue;

        if (type == typeof(bool))
        {
            if (value == "1")
                return true;
            else if (value == "0")
                return false;
            else
                throw new System.FormatException("Unable to cast '" + value + "' to bool");
        }
        else
        {
            return System.Convert.ChangeType(value, type);
        }
    }

    public static Datasheet<T> Load(string filename)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        string csv = File.ReadAllText(filename);
        MemberInfo[] members = FormatterServices.GetSerializableMembers(typeof(T));
        int expectedFieldCount = 0;
        T dummy = new T();
        foreach (MemberInfo member in members)
        {
            FieldInfo fi = (FieldInfo)member;
            if (fi.FieldType.IsArray)
            {
                expectedFieldCount += ((System.Collections.IList)fi.GetValue(dummy)).Count;
            }
            else
            {
                expectedFieldCount += 1;
            }
        }
        Datasheet<T> sheet = new Datasheet<T>();
        sheet.rows = new List<T>();
        var lines = csv.Split('\n');
        for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
        {
            string line = lines[lineIndex].Trim();
            if (line.Length == 0)
                continue;

            var fields = line.Split('\t');
            if (fields.Length != expectedFieldCount)
                throw new System.Exception("Field count mismatch " + typeof(T) + " (" + expectedFieldCount + " expected) at " + filename + ":" + (lineIndex + 1) + " (" + fields.Length + " fields)");

            if (lineIndex == 0)
                continue;

            T obj = new T();
            int memberIndex = 0;
            for (int fieldIndex = 0; fieldIndex < fields.Length; ++memberIndex)
            {
                MemberInfo member = members[memberIndex];
                FieldInfo fi = (FieldInfo)member;
                try
                {
                    if (fi.FieldType.IsArray)
                    {
                        var elementType = fi.FieldType.GetElementType();
                        var array = (System.Collections.IList)fi.GetValue(obj);
                        for (int i = 0; i < array.Count; ++i)
                        {
                            array[i] = CastValue(fields[fieldIndex], elementType, array[i]);
                            ++fieldIndex;
                        }
                    }
                    else
                    {
                        var value = CastValue(fields[fieldIndex], fi.FieldType, fi.GetValue(obj));
                        fi.SetValue(obj, value);
                        ++fieldIndex;
                    }
                }
                catch (System.Exception e)
                {
                    throw new System.Exception("Datasheet parsing error at " + filename + ":" + (lineIndex + 1) + " column " + (fieldIndex + 1) + " memberIndex " + memberIndex + " member " + member);
                }
            }
            sheet.rows.Add(obj);
        }
        Debug.Log("Load " + filename + " (" + sheet.rows.Count + " items, elapsed " + stopwatch.Elapsed.Milliseconds + " ms)");
        return sheet;
    }
}

[System.Serializable]
public class Obj
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
    public string _class;
    public string[] gear = new string[16];
    public string colormap;
    public string index;
    string eol;

    public static Datasheet<Obj> sheet = Datasheet<Obj>.Load(Application.streamingAssetsPath + "/d2/obj.txt");
    static Dictionary<long, Obj> lookup = new Dictionary<long, Obj>();

    static Obj()
    {
        foreach (Obj obj in sheet.rows)
        {
            lookup.Add(Key(obj.act, obj.type, obj.id), obj);
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

    static public Obj Find(int act, int type, int id)
    {
        Obj obj = null;
        lookup.TryGetValue(Key(act, type, id), out obj);
        return obj;
    }
}

[System.Serializable]
public class ObjectInfo
{
    public string name;
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

    public static Datasheet<ObjectInfo> sheet = Datasheet<ObjectInfo>.Load(Application.streamingAssetsPath + "/d2/data/global/excel/objects.txt");
}

[System.Serializable]
public class MonStat
{
    const int DifficultyCount = 3;

    public string id;
    public int hcIndex;
    public string baseId;
    public string nextInClass;
    public string transLvl;
    public string nameStr;
    public string monStatEx;
    public string monProp;
    public string monType;
    public string ai;
    public string descStr;
    public string code;
    public bool enabled;
    public string rangedType;
    public string placeSpawn;
    public string spawn;
    public string spawnX;
    public string spawnY;
    public string spawnMode;
    public string minion1;
    public string minion2;
    public string SetBoss;
    public string BossXfer;
    public string PartyMin;
    public string PartyMax;
    public string MinGrp;
    public string MaxGrp;
    public string sparsePopulate;
    public int speed;
    public int runSpeed;
    public string Rarity;
    public string[] level = new string[DifficultyCount];
    public string MonSound;
    public string UMonSound;
    public string threat;
    public string[] aidel = new string[DifficultyCount];
    public string[] aidist = new string[DifficultyCount];
    public string[] aip1 = new string[8 * DifficultyCount];
    public string MissA1;
    public string MissA2;
    public string MissS1;
    public string MissS2;
    public string MissS3;
    public string MissS4;
    public string MissC;
    public string MissSQ;
    public string Align;
    public string isSpawn;
    public string isMelee;
    public bool npc;
    public string interact;
    public string inventory;
    public string inTown;
    public string lUndead;
    public string hUndead;
    public string demon;
    public string flying;
    public string opendoors;
    public string boss;
    public string primeevil;
    public string killable;
    public string switchai;
    public string noAura;
    public string nomultishot;
    public string neverCount;
    public string petIgnore;
    public string deathDmg;
    public string genericSpawn;
    public string zoo;
    public string SendSkills;

    public string Skill1;
    public string Sk1mode;
    public string Sk1lvl;
    public string[] remainingSkillsInfo = new string[3 * 7];

    public string[] drain = new string[DifficultyCount];
    public string[] coldEffect = new string[DifficultyCount];

    public string ResDm;
    public string ResMa;
    public string ResFi;
    public string ResLi;
    public string ResCo;
    public string ResPo;
    public string[] remainingResInfo = new string[6 * (DifficultyCount - 1)];

    public string DamageRegen;
    public string skillDamage;
    public string noRatio;
    public string NoShldBlock;
    public string[] toBlock = new string[DifficultyCount];
    public string Crit;

    public string minHP;
    public string maxHP;
    public string AC;
    public string Exp;
    public string A1MinD;
    public string A1MaxD;
    public string A1TH;
    public string A2MinD;
    public string A2MaxD;
    public string A2TH;
    public string S1MinD;
    public string S1MaxD;
    public string S1TH;
    public string[] repeatedStruct = new string[13 * (DifficultyCount - 1)];

    public string[] elementalDamage = new string[3 * (2 + 4 * DifficultyCount)];
    public string[] treasureClass = new string[4 * 3];
    public string TCQuestId;
    public string TCQuestCP;
    public string SplEndDeath;
    public string SplGetModeChart;
    public string SplEndGeneric;
    public string SplClientEnd;
    string eol;

    public static Datasheet<MonStat> sheet = Datasheet<MonStat>.Load(Application.streamingAssetsPath + "/d2/data/global/excel/monstats.txt");
    static Dictionary<string, MonStat> stats = new Dictionary<string, MonStat>();

    static MonStat()
    {
        foreach(MonStat stat in sheet.rows)
        {
            if (stats.ContainsKey(stat.id))
            {
                stats.Remove(stat.id);
            }
            stats.Add(stat.id.ToLower(), stat);
        }
    }

    public static MonStat Find(int act, int id)
    {
        MonPreset preset = MonPreset.Find(act, id);
        if (preset != null)
        {
            if (stats.ContainsKey(preset.place))
                return stats[preset.place];
            else
                return null;
        }
        else
        {
            return sheet.rows[id];
        }
    }
}

[System.Serializable]
public class MonPreset
{
    const int ActCount = 5;

    public int act;
    public string place;

    public static Datasheet<MonPreset> sheet = Datasheet<MonPreset>.Load(Application.streamingAssetsPath + "/d2/data/global/excel/MonPreset.txt");
    static List<MonPreset>[] presets = new List<MonPreset>[ActCount + 1];

    static MonPreset()
    {
        for(int act = 0; act < presets.Length; ++act)
        {
            presets[act] = new List<MonPreset>();
        }

        for(int i = 0; i < sheet.rows.Count; ++i)
        {
            MonPreset preset = sheet.rows[i];
            preset.place = preset.place.ToLower();
            presets[preset.act].Add(preset);
        }
    }

    public static MonPreset Find(int act, int id)
    {
        var actPresets = presets[act];
        if (id < actPresets.Count)
        {
            return actPresets[id];
        }
        return null;
    }
}