using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;


public struct Datasheet<T> where T : new()
{
    public List<T> rows;

    public static Datasheet<T> Load(string filename)
    {
        string csv = File.ReadAllText(filename);
        MemberInfo[] members = FormatterServices.GetSerializableMembers(typeof(T));
        Datasheet<T> sheet = new Datasheet<T>();
        sheet.rows = new List<T>();
        var lines = csv.Split('\n');
        for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
        {
            string line = lines[lineIndex].Trim();
            if (line.Length == 0)
                continue;

            var fields = line.Split('\t');
            if (fields.Length != members.Length)
                throw new System.Exception("Field count mismatch " + typeof(T) + " (" + members.Length + " members) at " + filename + ":" + (lineIndex + 1) + " (" + fields.Length + " fields)");

            if (lineIndex == 0)
                continue;

            T obj = new T();
            for (int fieldIndex = 0; fieldIndex < fields.Length; ++fieldIndex)
            {
                FieldInfo fi = (FieldInfo)members[fieldIndex];
                var value = System.Convert.ChangeType(fields[fieldIndex], fi.FieldType);
                fi.SetValue(obj, value);
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
    public string objectId;
    public string monstatId;
    public string direction;
    public string _base;
    public string token;
    public string mode;
    public string _class;
    public string HD;
    public string TR;
    public string LG;
    public string RA;
    public string LA;
    public string RH;
    public string LH;
    public string SH;
    public string S1;
    public string S2;
    public string S3;
    public string S4;
    public string S5;
    public string S6;
    public string S7;
    public string S8;
    public string colormap;
    public string index;
    public string eol;

    public static Datasheet<Obj> sheet = Datasheet<Obj>.Load("Assets/d2/obj.txt");
    static Dictionary<long, Obj> lookup = new Dictionary<long, Obj>();

    static Obj()
    {
        foreach(Obj obj in sheet.rows)
            lookup.Add(Key(obj.act, obj.type, obj.id), obj);
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