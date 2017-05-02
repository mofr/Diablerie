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
        var nodes = new List<Node>();

        for(int i = 0; i < 100; i += 3)
        {
            var treasureClass = new TreasureClass();
            treasureClass.name = "armo" + i;
            treasureClass.picks = 1;
            byName.Add(treasureClass.name, treasureClass);
            nodes.Clear();
            foreach (var armor in ArmorInfo.sheet)
            {
                if (armor.level > i && armor.level <= i + 3)
                {
                    var node = new Node();
                    node.code = armor.code;
                    node.prob = 1;
                    nodes.Add(node);
                    treasureClass.probSum += node.prob;
                }
            }
            treasureClass.nodeArray = nodes.ToArray();
        }

        for (int i = 0; i < 100; i += 3)
        {
            var treasureClass = new TreasureClass();
            treasureClass.name = "weap" + i;
            treasureClass.picks = 1;
            byName.Add(treasureClass.name, treasureClass);
            nodes.Clear();
            foreach (var weapon in WeaponInfo.sheet)
            {
                if (weapon.level > i && weapon.level <= i + 3)
                {
                    var node = new Node();
                    node.code = weapon.code;
                    node.prob = 1;
                    nodes.Add(node);
                    treasureClass.probSum += node.prob;
                }
            }
            treasureClass.nodeArray = nodes.ToArray();
        }

        foreach (TreasureClass tc in sheet)
        {
            if (tc.name == null)
                continue;
            if (byName.ContainsKey(tc.name))
                continue;
            byName.Add(tc.name, tc);
        }

        foreach (TreasureClass tc in sheet)
        {
            if (tc.name == null)
                continue;
            tc.probSum = 0;
            for(int i = 0; i < tc.nodeArray.Length; ++i)
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

    [System.Serializable]
    public struct Node
    {
        public string code;
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
    public Node[] nodeArray;
    public int sumItems;
    public int totalProb;
    public float dropChance;
    public string term;

    [System.NonSerialized]
    public int probSum;
}
