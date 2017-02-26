using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;


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
    public string[] layers = new string[16];
    public string colormap;
    public string index;
    public string eol;

    [System.NonSerialized]
    public int modeIndex;

    public static Datasheet<Obj> sheet = Datasheet<Obj>.Load("Assets/d2/obj.txt");
    static Dictionary<long, Obj> lookup = new Dictionary<long, Obj>();

    public static readonly string[] ShortModeNames = { "NU", "OP", "ON", "S1", "S2", "S3", "S4", "S5" };

    static Obj()
    {
        foreach (Obj obj in sheet.rows)
        {
            obj.modeIndex = System.Array.IndexOf(ShortModeNames, obj.mode);
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

    public static Datasheet<ObjectInfo> sheet = Datasheet<ObjectInfo>.Load("Assets/d2/data/global/excel/objects.txt");
}