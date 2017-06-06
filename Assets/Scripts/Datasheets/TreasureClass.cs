using System.Collections.Generic;

[System.Serializable]
public class TreasureClass
{
    public static List<TreasureClass> sheet = Datasheet.Load<TreasureClass>("data/global/excel/TreasureClassEx.txt");
    static Dictionary<string, TreasureClass> byName = new Dictionary<string, TreasureClass>();

    public static TreasureClass Find(string name)
    {
        if (name == null)
            return null;
        return byName.GetValueOrDefault(name);
    }

    public TreasureClass Upgraded(int targetLevel)
    {
        if (group == -1)
            return this;

        int i = index;
        while(i < sheet.Count - 1 && sheet[i + 1].level <= targetLevel && sheet[i + 1].group == group)
        {
            ++i;
        }
        return sheet[i];
    }

    static TreasureClass()
    {
        GenerateFromItemTypes();

        for (int i = 0; i < sheet.Count; ++i)
        {
            TreasureClass tc = sheet[i];
            if (tc.name == null)
                continue;
            if (byName.ContainsKey(tc.name))
                continue;
            byName.Add(tc.name, tc);
            tc.index = i;
        }

        foreach (TreasureClass tc in sheet)
        {
            if (tc.name == null)
                continue;
            tc.probSum = 0;
            for (int i = 0; i < tc.nodeArray.Length; ++i)
            {
                var node = tc.nodeArray[i];
                if (node.code == null)
                    break;
                if (node.code.StartsWith("\"gld"))
                    node.code = "gld"; // todo correctly parse strings like "gld,mul=123"
                tc.probSum += node.prob;
                tc.nodeArray[i] = node;
            }
        }
    }

    private static void GenerateFromItemTypes()
    {
        var nodes = new List<Node>();

        foreach (var type in ItemType.sheet)
        {
            if (!type.treasureClass)
                continue;

            for (int i = 0; i < 100; i += 3)
            {
                var treasureClass = new TreasureClass();
                treasureClass.name = type.code + i;
                treasureClass.picks = 1;
                byName.Add(treasureClass.name, treasureClass);
                nodes.Clear();
                foreach (var item in ItemInfo.all)
                {
                    if (item.level > i && item.level <= i + 3 && item.HasType(type))
                    {
                        var node = new Node();
                        node.code = item.code;
                        node.prob = 1;
                        nodes.Add(node);
                        treasureClass.probSum += node.prob;
                    }
                }
                treasureClass.nodeArray = nodes.ToArray();
            }
        }
    }

    [System.Serializable]
    public struct Node
    {
        public string code;
        public int prob;
    }

    public string name;
    public int group = -1;
    public int level;
    public int picks;
    public int unique;
    public int set;
    public int rare;
    public int magic;
    public int noDrop;
    [Datasheet.Sequence(length = 10)]
    public Node[] nodeArray;
    public int sumItems;
    public int totalProb;
    public float dropChance;
    public string term;

    [System.NonSerialized]
    public int probSum;

    [System.NonSerialized]
    public int index;
}
