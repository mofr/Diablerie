using UnityEngine;

public class Equipment : MonoBehaviour
{
    Character character;
    public Item[] items;

    public delegate void OnUpdateHandler();
    public event OnUpdateHandler OnUpdate;

    static string[] armorTypes = new string[] { "LIT", "MED", "HVY" };
    
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
            if (item.info.armor != null && item.info.armor.rArm != -1)
            {
                character.gear[System.Array.IndexOf(COF.layerNames, "RA")] = armorTypes[item.info.armor.rArm];
                character.gear[System.Array.IndexOf(COF.layerNames, "LA")] = armorTypes[item.info.armor.lArm];
                character.gear[System.Array.IndexOf(COF.layerNames, "TR")] = armorTypes[item.info.armor.torso];
                character.gear[System.Array.IndexOf(COF.layerNames, "LG")] = armorTypes[item.info.armor.legs];
                character.gear[System.Array.IndexOf(COF.layerNames, "S1")] = armorTypes[item.info.armor.rSPad];
                character.gear[System.Array.IndexOf(COF.layerNames, "S2")] = armorTypes[item.info.armor.lSPad];
            }
            else
            {
                if (item.info.component < character.gear.Length)
                    character.gear[item.info.component] = item.info.alternateGfx;
                if (item.info.weapon != null)
                    character.weaponClass = item.info.weapon.wClass;
            }
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
