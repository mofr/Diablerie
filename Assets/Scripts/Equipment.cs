using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Item[] items;

    public delegate void OnUpdateHandler();
    public event OnUpdateHandler OnUpdate;

    public Item Equip(Item item)
    {
        Item previous = null;
        if (items[item.info.type.bodyLoc1] != null)
        {
            previous = items[item.info.type.bodyLoc2];
            items[item.info.type.bodyLoc2] = item;
        }
        else
        {
            previous = items[item.info.type.bodyLoc1];
            items[item.info.type.bodyLoc1] = item;
        }

        if (OnUpdate != null)
            OnUpdate();

        return previous;
    }

    void Awake()
    {
        items = new Item[BodyLoc.sheet.Count];
    }
}
