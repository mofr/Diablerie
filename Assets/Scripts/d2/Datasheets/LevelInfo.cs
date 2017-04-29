using System.Collections.Generic;

[System.Serializable]
public class LevelInfo
{
    public string name;
    public int id;
    public int pal;
    public int act;
    public int questFlag;
    public int questFlagEx;
    public int layer;
    public int sizeX;
    public int sizeY;
    public int sizeXNightmare;
    public int sizeYNightmare;
    public int sizeXHell;
    public int sizeYHell;
    public int offsetX;
    public int offsetY;
    public int depend;
    public int teleport;
    public int rain;
    public int mud;
    public int noPer;
    public int LOSDraw;
    public int floorFilter;
    public int blankScreen;
    public int drawEdges;
    public int isInside;
    public int drlgType;
    public int levelTypeIndex;
    public int subType;
    public int subTheme;
    public int subWaypoint;
    public int subShrine;
    public int[] vis = new int[8];
    public int[] warp = new int[8];
    public int intensity;
    public int red;
    public int green;
    public int blue;
    public int portal;
    public int position;
    public bool saveMonsters;
    public int quest;
    public int warpDist;
    public int[] monLvl = new int[3];
    public int[] monLvlEx = new int[3];
    public int[] monDen = new int[3];
    public int monUMin;
    public int monUMax;
    public int monUMinNightmare;
    public int monUMaxNightmare;
    public int monUMinHell;
    public int monUMaxHell;
    public int monWndr;
    public int monSpcWalk;
    public int numMon;
    public string[] _monsters = new string[10];
    public int rangedspawn;
    public string[] nMonsters = new string[10];
    public string[] uMonsters = new string[10];
    public string[] cmon = new string[4];
    public int[] cpct = new int[4];
    public int[] camt = new int[4];
    public int themes;
    public int soundEnv;
    public int waypoint;
    public string levelName;
    public string levelWarp;
    public string _entryFile;
    public int[] objGrp = new int[8];
    public int[] objPrb = new int[8];
    public bool beta;

    [System.NonSerialized]
    public LevelType type;

    [System.NonSerialized]
    public LevelPreset preset;

    [System.NonSerialized]
    public string entryFile;

    [System.NonSerialized]
    public List<string> monsters = new List<string>();

    public static Datasheet<LevelInfo> sheet = Datasheet<LevelInfo>.Load("data/global/excel/Levels.txt");
    static Dictionary<string, LevelInfo> nameIndex = new Dictionary<string, LevelInfo>();
    static Dictionary<int, LevelInfo> idMap = new Dictionary<int, LevelInfo>();

    static LevelInfo()
    {
        foreach(var levelInfo in sheet.rows)
        {
            if (levelInfo.id == 0)
                continue;
            levelInfo.type = LevelType.sheet.rows[levelInfo.levelTypeIndex];
            levelInfo.preset = LevelPreset.Find(levelInfo.id);
            levelInfo.entryFile = @"data\local\ui\eng\act" + (levelInfo.act + 1) + @"\" + levelInfo._entryFile + ".dc6";
            foreach(string mon in levelInfo._monsters)
            {
                if (mon != null && mon != "")
                    levelInfo.monsters.Add(mon);
            }
            nameIndex.Add(levelInfo.name, levelInfo);
            idMap.Add(levelInfo.id, levelInfo);
        }
    }

    public static LevelInfo Find(string name)
    {
        return nameIndex.GetValueOrDefault(name, null);
    }

    public static LevelInfo Find(int id)
    {
        return idMap.GetValueOrDefault(id, null);
    }
}
