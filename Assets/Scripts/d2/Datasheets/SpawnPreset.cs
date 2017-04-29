using System.Collections.Generic;

[System.Serializable]
public class SpawnPreset
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
    public string weaponClass;
    public string[] gear = new string[16];
    public string colormap;
    public string index;
    string eol;

    public static Datasheet<SpawnPreset> sheet = Datasheet<SpawnPreset>.Load("/obj.txt");
    static Dictionary<long, SpawnPreset> lookup = new Dictionary<long, SpawnPreset>();

    static SpawnPreset()
    {
        foreach (SpawnPreset obj in sheet.rows)
        {
            lookup.Add(Key(obj.act - 1, obj.type, obj.id), obj);
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

    static public SpawnPreset Find(int act, int type, int id)
    {
        SpawnPreset obj = null;
        lookup.TryGetValue(Key(act, type, id), out obj);
        return obj;
    }
}
