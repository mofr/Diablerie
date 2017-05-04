using System.Collections.Generic;

[System.Serializable]
public class LevelType
{
    public string name;
    public int id;
    [Datasheet.Sequence(length = 32)]
    public string[] files;
    public bool beta;
    public int act;

    [System.NonSerialized]
    public List<string> dt1Files = new List<string>();

    public static List<LevelType> sheet = Datasheet.Load<LevelType>("data/global/excel/LvlTypes.txt");

    static LevelType()
    {
        foreach (var levelType in sheet)
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
