using System.Collections.Generic;

[System.Serializable]
public class LevelPreset
{
    public string name;
    public int def;
    public int levelId;
    public bool populate;
    public bool logicals;
    public bool outdoors;
    public bool animate;
    public bool killEdge;
    public bool fillBlanks;
    public int sizeX;
    public int sizeY;
    public int autoMap;
    public bool scan;
    public string pops;
    public string popPad;
    public int fileCount;
    public string[] files = new string[6];
    public int dt1Mask;
    public bool beta;

    [System.NonSerialized]
    public List<string> ds1Files = new List<string>();

    public static Datasheet<LevelPreset> sheet = Datasheet<LevelPreset>.Load("data/global/excel/LvlPrest.txt");
    static Dictionary<int, LevelPreset> levelIdMap = new Dictionary<int, LevelPreset>();
    static Dictionary<string, LevelPreset> nameMap = new Dictionary<string, LevelPreset>();

    static LevelPreset()
    {
        foreach (var preset in sheet.rows)
        {
            if (preset.levelId != 0)
                levelIdMap.Add(preset.levelId, preset);
            if (!nameMap.ContainsKey(preset.name))
                nameMap.Add(preset.name, preset);
            foreach (var filename in preset.files)
            {
                if (filename != "0" && filename != null)
                    preset.ds1Files.Add(@"data\global\tiles\" + filename.Replace("/", @"\"));
            }
        }
    }

    static public LevelPreset Find(int levelId)
    {
        return levelIdMap.GetValueOrDefault(levelId);
    }

    static public LevelPreset Find(string name)
    {
        return nameMap.GetValueOrDefault(name);
    }
}
