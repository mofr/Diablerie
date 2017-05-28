using System.Collections.Generic;

[System.Serializable]
public class LevelMazeInfo
{
    public static List<LevelMazeInfo> sheet = Datasheet.Load<LevelMazeInfo>("data/global/excel/LvlMaze.txt");
    static Dictionary<int, LevelMazeInfo> map = new Dictionary<int, LevelMazeInfo>();

    public static LevelMazeInfo Find(int levelId)
    {
        return map.GetValueOrDefault(levelId);
    }

    static LevelMazeInfo()
    {
        foreach(var info in sheet)
        {
            if (info.levelId == -1)
                continue;

            map.Add(info.levelId, info);
        }
    }
    
    public string name;
    public int levelId = -1;
    [Datasheet.Sequence(length = 3)]
    public int[] rooms;
    public int sizeX;
    public int sizeY;
    public int merge;
    public bool beta;
}
