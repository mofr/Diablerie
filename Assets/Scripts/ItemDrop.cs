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

    public void Drop(string code, Vector3 pos)
    {
        var treasureClass = TreasureClass.Find(code);
        if (treasureClass == null)
        {
            var pickup = World.SpawnItem(code, pos);
            if (pickup == null)
                Debug.LogError("Item wasn't spawned: " + code);
            return;
        }

        int noDrop = treasureClass.noDrop / 4; // division is temporary just to increase drop for debugging purposes
        for (int i = 0; i < treasureClass.picks; ++i)
        {
            int sample = Random.Range(0, treasureClass.probSum + noDrop);
            if (sample < noDrop)
                continue;
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
    }
}
