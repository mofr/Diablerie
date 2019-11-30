using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
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
        [Datasheet.Sequence(length = 8)]
        public int[] vis;
        [Datasheet.Sequence(length = 8)]
        public int[] warp;
        public int intensity;
        public int red;
        public int green;
        public int blue;
        public int portal;
        public int position;
        public bool saveMonsters;
        public int quest;
        public int warpDist;
        [Datasheet.Sequence(length = 3)]
        public int[] monLvl;
        [Datasheet.Sequence(length = 3)]
        public int[] monLvlEx;
        [Datasheet.Sequence(length = 3)]
        public int[] monDen;
        public int monUMin;
        public int monUMax;
        public int monUMinNightmare;
        public int monUMaxNightmare;
        public int monUMinHell;
        public int monUMaxHell;
        public int monWndr;
        public int monSpcWalk;
        public int numMon;
        [Datasheet.Sequence(length = 10)]
        public string[] _monsters;
        public int rangedspawn;
        [Datasheet.Sequence(length = 10)]
        public string[] nMonsters;
        [Datasheet.Sequence(length = 10)]
        public string[] uMonsters;
        [Datasheet.Sequence(length = 4)]
        public string[] cmon;
        [Datasheet.Sequence(length = 4)]
        public int[] cpct;
        [Datasheet.Sequence(length = 4)]
        public int[] camt;
        public int themes;
        public int soundEnvId;
        public int waypoint;
        public string levelName;
        public string levelWarp;
        public string _entryFile;
        [Datasheet.Sequence(length = 8)]
        public int[] objGrp;
        [Datasheet.Sequence(length = 8)]
        public int[] objPrb;
        public bool beta;

        [System.NonSerialized]
        public LevelType type;

        [System.NonSerialized]
        public LevelPreset preset;

        [System.NonSerialized]
        public string entryFile;

        [System.NonSerialized]
        public List<string> monsters = new List<string>();

        [System.NonSerialized]
        public SoundEnvironment soundEnv;

        [System.NonSerialized]
        public LevelMazeInfo maze;

        public static List<LevelInfo> sheet = Datasheet.Load<LevelInfo>("data/global/excel/Levels.txt");
        public static List<List<LevelInfo>> byAct = new List<List<LevelInfo>>();
        static Dictionary<string, LevelInfo> nameIndex = new Dictionary<string, LevelInfo>();
        static Dictionary<int, LevelInfo> idMap = new Dictionary<int, LevelInfo>();

        static LevelInfo()
        {
            foreach(var levelInfo in sheet)
            {
                if (levelInfo.id == 0)
                    continue;
                levelInfo.type = LevelType.sheet[levelInfo.levelTypeIndex];
                levelInfo.preset = LevelPreset.Find(levelInfo.id);
                levelInfo.entryFile = @"data\local\ui\eng\act" + (levelInfo.act + 1) + @"\" + levelInfo._entryFile + ".dc6";
                foreach(string mon in levelInfo._monsters)
                {
                    if (mon != null && mon != "")
                        levelInfo.monsters.Add(mon);
                }
                levelInfo.soundEnv = SoundEnvironment.Find(levelInfo.soundEnvId);
                levelInfo.maze = LevelMazeInfo.Find(levelInfo.id);
                nameIndex.Add(levelInfo.name, levelInfo);
                idMap.Add(levelInfo.id, levelInfo);
                while (byAct.Count <= levelInfo.act)
                    byAct.Add(new List<LevelInfo>());
                byAct[levelInfo.act].Add(levelInfo);
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
}
