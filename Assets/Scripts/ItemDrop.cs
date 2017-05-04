using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    void OnEnable()
    {
        Character.OnDeath += OnCharacterDeath;
    }

    private void OnCharacterDeath(Character target, Character killer)
    {
        if (target.monStat == null)
            return;

        Drop(target.monStat.treasureClass[0], target.transform.position);
    }

    public static void Drop(string code, Vector3 pos)
    {
        var treasureClass = TreasureClass.Find(code);
        if (treasureClass == null)
        {
            var item = Item.Create(code);
            if (item == null)
                Debug.LogWarning("Item wasn't spawned: " + code);
            else
                Pickup.Create(pos, item);
            return;
        }

        int noDrop = 0; // value is temporary just to increase drop for debugging purposes, should be treasureClass.noDrop
        for (int i = 0; i < treasureClass.picks; ++i)
        {
            int sample = Random.Range(0, treasureClass.probSum + noDrop);
            foreach (var node in treasureClass.nodeArray)
            {
                if (node.code == null)
                    break;

                if (sample < node.prob)
                {
                    Drop(node.code, pos);
                    break;
                }

                sample -= node.prob;
            }
        }

        for (int i = 0; i < -treasureClass.picks; ++i)
        {
            var node = treasureClass.nodeArray[i];
            if (node.code == null)
                break;
            for (int j = 0; j < node.prob; ++j)
            {
                Drop(node.code, pos);
            }
        }
    }
}
