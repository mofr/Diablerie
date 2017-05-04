using System.Collections.Generic;

[System.Serializable]
public class LevelWarpInfo
{
    public string name;
    public int id;
    public int selectX;
    public int selectY;
    public int selectDX;
    public int selectDY;
    public int exitWalkX;
    public int exitWalkY;
    public int offsetX;
    public int offsetY;
    public int litVersion;
    public int tiles;
    public string direction;
    public int beta;

    [System.NonSerialized]
    public Warp instance;

    public static List<LevelWarpInfo> sheet = Datasheet.Load<LevelWarpInfo>("data/global/excel/LvlWarp.txt");
    static Dictionary<int, LevelWarpInfo> idMap = new Dictionary<int, LevelWarpInfo>();

    static LevelWarpInfo()
    {
        foreach(var warpInfo in sheet)
        {
            idMap[warpInfo.id] = warpInfo;
        }
    }

    static public LevelWarpInfo Find(int id)
    {
        return idMap[id];
    }
}
