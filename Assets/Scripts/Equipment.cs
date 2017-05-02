using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Item[] items;

    public void Equip(Item item)
    {
        if (items[item.info.type.bodyLoc1] != null)
        {
            if (items[item.info.type.bodyLoc2] != null)
            {
                Pickup.Create(transform.position, items[item.info.type.bodyLoc2]);
            }
            items[item.info.type.bodyLoc2] = item;
        }
        else
        {
            items[item.info.type.bodyLoc1] = item;
        }
    }

    void Start()
    {
        items = new Item[BodyLoc.sheet.Count];
    }
}
