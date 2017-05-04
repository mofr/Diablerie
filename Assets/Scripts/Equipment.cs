using UnityEngine;

public class Equipment : MonoBehaviour
{
    Character character;
    public Item[] items;

    public delegate void OnUpdateHandler();
    public event OnUpdateHandler OnUpdate;
    
    public Item Equip(Item item)
    {
        if (!item.info.type.body)
            return null;

        Item previous = null;
        if (items[item.info.type.bodyLoc1] != null)
        {
            previous = Equip(item, item.info.type.bodyLoc2);
        }
        else
        {
            previous = Equip(item, item.info.type.bodyLoc1);
        }

        if (OnUpdate != null)
            OnUpdate();

        return previous;
    }

    public Item Equip(Item item, int loc)
    {
        Item previous = items[loc];
        items[loc] = item;

        if (item != null)
        {
            if (item.info.component < character.gear.Length)
                character.gear[item.info.component] = item.info.alternateGfx;
            if (item.info.weapon != null)
                character.weaponClass = item.info.weapon.wClass;
        }

        if (OnUpdate != null)
            OnUpdate();

        return previous;
    }

    void Awake()
    {
        character = GetComponent<Character>();
        items = new Item[BodyLoc.sheet.Count];
    }
}
