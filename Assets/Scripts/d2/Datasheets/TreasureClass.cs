using System.Collections.Generic;

[System.Serializable]
public class TreasureClass
{
    public static List<TreasureClass> sheet = Datasheet.Load<TreasureClass>("data/global/excel/TreasureClassEx.txt");
    static Dictionary<string, TreasureClass> byName = new Dictionary<string, TreasureClass>();

    public static TreasureClass Find(string name)
    {
        return byName.GetValueOrDefault(name);
    }

    static TreasureClass()
    {
        foreach (TreasureClass tc in sheet)
        {
            if (tc.name == null)
                continue;
            if (byName.ContainsKey(tc.name))
                continue;
            byName.Add(tc.name, tc);
        }
    }

    [System.Serializable]
    public struct Record
    {
        public string item;
        public int prob;
    }

    public string name;
    public int group;
    public int level;
    public int picks;
    public int unique;
    public int set;
    public int rare;
    public int magic;
    public int noDrop;
    [Datasheet.Sequence(length = 10)]
    public Record[] sub;
    public int sumItems;
    public int totalProb;
    public float dropChance;
    public string term;
}
