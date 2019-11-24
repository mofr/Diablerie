using System.Collections.Generic;

[System.Serializable]
public class OverlayInfo
{
    public string id;
    public string filename;
    [Datasheet.Sequence(length = 3)]
    public string[] unused;
    public bool preDraw;
    [Datasheet.Sequence(length = 4)]
    public string[] unused2;
    public int xOffset;
    public int yOffset;
    public int height1;
    public int height2;
    public int height3;
    public int height4;
    public int fps;
    public string loopWaitTime;
    public int trans;
    public int initRadius;
    public int radius;
    public int red;
    public int green;
    public int blue;
    public int numDirections;
    public bool localBlood;

    [System.NonSerialized]
    public string spritesheetFilename;

    public static List<OverlayInfo> sheet = Datasheet.Load<OverlayInfo>("data/global/excel/Overlay.txt");
    static Dictionary<string, OverlayInfo> map;

    static OverlayInfo()
    {
        map = new Dictionary<string, OverlayInfo>(sheet.Count);
        foreach (var row in sheet)
        {
            if (row.filename == null)
                continue;

            row.spritesheetFilename = @"data\global\overlays\" + row.filename;
            map.Add(row.id, row);
        }
    }

    public static OverlayInfo Find(string id)
    {
        if (id == null)
            return null;
        return map.GetValueOrDefault(id);
    }
}
