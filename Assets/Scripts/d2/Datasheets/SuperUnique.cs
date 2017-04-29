using System.Collections.Generic;

[System.Serializable]
public class SuperUnique
{
    public static Datasheet<SuperUnique> sheet = Datasheet<SuperUnique>.Load("data/global/excel/SuperUniques.txt");
    static Dictionary<string, SuperUnique> map = new Dictionary<string, SuperUnique>();

    public static SuperUnique Find(string key)
    {
        return map.GetValueOrDefault(key);
    }

    static SuperUnique()
    {
        foreach (var row in sheet.rows)
        {
            row.monStat = MonStat.Find(row.monStatId);
            row.name = Translation.Find(row.nameStr);
            map.Add(row.superUnique, row);
        }
    }

    public string superUnique;
    public string nameStr;
    public string monStatId;
    public int hcIdx;
    public string monSound;
    public int mod1;
    public int mod2;
    public int mod3;
    public int minGrp;
    public int maxGrp;
    public int eClass;
    public bool autoPos;
    public int stacks;
    public bool replacable;
    public int[] uTrans = new int[3];
    public string[] treasureClass = new string[3];
    public string eol;

    [System.NonSerialized]
    public MonStat monStat;

    [System.NonSerialized]
    public string name;
}
