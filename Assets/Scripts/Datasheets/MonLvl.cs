using System.Collections.Generic;

[System.Serializable]
public class MonLvl
{
    public static List<MonLvl> sheet = Datasheet.Load<MonLvl>("data/global/excel/MonLvl.txt");
    static Dictionary<int, MonLvl> map = new Dictionary<int, MonLvl>();

    public static MonLvl Find(int level)
    {
        return map.GetValueOrDefault(level);
    }

    static MonLvl()
    {
        foreach(var monLvl in sheet)
        {
            map.Add(monLvl.level, monLvl);
        }
    }
    
    public int level;
    [Datasheet.Sequence(length = 6)]
    public int[] armor;
    [Datasheet.Sequence(length = 6)]
    public int[] toHit;
    [Datasheet.Sequence(length = 6)]
    public int[] hp;
    [Datasheet.Sequence(length = 6)]
    public int[] damage;
    [Datasheet.Sequence(length = 6)]
    public int[] experience;
}
