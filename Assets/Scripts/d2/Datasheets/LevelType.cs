using System.Collections.Generic;

[System.Serializable]
public class LevelType
{
    public string name;
    public int id;
    public string[] files = new string[32];
    public bool beta;
    public int act;

    [System.NonSerialized]
    public List<string> dt1Files = new List<string>();

    public static Datasheet<LevelType> sheet = Datasheet<LevelType>.Load("data/global/excel/LvlTypes.txt");

    static LevelType()
    {
        foreach (var levelType in sheet.rows)
        {
            foreach (var file in levelType.files)
            {
                if (file == "0" || file == null)
                    continue;

                levelType.dt1Files.Add(@"data\global\tiles\" + file.Replace("/", @"\"));
            }
        }
    }
}
