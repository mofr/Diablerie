using UnityEngine;

public class Equipment : MonoBehaviour
{
    public Item[] items;

    public delegate void OnUpdateHandler();
    public event OnUpdateHandler OnUpdate;

    Character character;
    COFAnimator animator;

    static string[] armorTypes = new string[] { "LIT", "MED", "HVY" };
    static string[] defaultEquip = new string[] { "LIT", "LIT", "LIT", "LIT", "LIT", "", "", "", "LIT", "LIT", "", "", "", "", "", "" };
    static Item[] unequippedItems = new Item[2];

    public bool Equip(Item item)
    {
        if (!item.identified)
            return false;

        if (!item.info.type.body)
            return false;
        
        if (items[item.info.type.bodyLoc1] != null || items[item.info.type.bodyLoc2] != null)
            return false;
        Equip(item, item.info.type.bodyLoc1);

        if (OnUpdate != null)
            OnUpdate();

        return true;
    }

    public Item[] Equip(Item item, int loc)
    {
        unequippedItems[0] = null;
        unequippedItems[1] = null;

        if (items[loc] != null)
            unequippedItems[0] = items[loc];

        if (item != null)
        {
            int loc2 = loc == item.info.type.bodyLoc1 ? item.info.type.bodyLoc2 : item.info.type.bodyLoc1;
            if (items[loc2] != null)
            {
                bool pop = item.info.weapon != null && (items[loc2].info.weapon != null || item.info.weapon.twoHanded);
                if (pop)
                {
                    unequippedItems[1] = items[loc2];
                    items[loc2] = null;
                }
            }
        }

        items[loc] = item;

        UpdateAnimator();

        if (OnUpdate != null)
            OnUpdate();

        return unequippedItems;
    }

    public Item GetWeapon()
    {
        foreach(var item in items)
        {
            if (item != null && item.info.weapon != null)
                return item;
        }

        return null;
    }

    void UpdateAnimator()
    {
        character.weaponClass = "HTH";
        var equip = animator.equip;
        if (equip == null)
            equip = new string[defaultEquip.Length];
        System.Array.Copy(defaultEquip, equip, defaultEquip.Length);
        foreach(var item in items)
        {
            if (item == null)
                continue;
            if (item.info.armor != null && item.info.armor.rArm != -1)
            {
                equip[System.Array.IndexOf(COF.layerNames, "RA")] = armorTypes[item.info.armor.rArm];
                equip[System.Array.IndexOf(COF.layerNames, "LA")] = armorTypes[item.info.armor.lArm];
                equip[System.Array.IndexOf(COF.layerNames, "TR")] = armorTypes[item.info.armor.torso];
                equip[System.Array.IndexOf(COF.layerNames, "LG")] = armorTypes[item.info.armor.legs];
                equip[System.Array.IndexOf(COF.layerNames, "S1")] = armorTypes[item.info.armor.rSPad];
                equip[System.Array.IndexOf(COF.layerNames, "S2")] = armorTypes[item.info.armor.lSPad];
            }
            else
            {
                if (item.info.component >= 0 && item.info.component < equip.Length)
                    equip[item.info.component] = item.info.alternateGfx;
                if (item.info.weapon != null)
                {
                    if (item.info.weapon.twoHanded)
                        character.weaponClass = item.info.weapon.twoHandedWClass;
                    else
                        character.weaponClass = item.info.weapon.wClass;
                }
            }
        }
        animator.equip = equip;
    }

    void Awake()
    {
        character = GetComponent<Character>();
        animator = GetComponent<COFAnimator>();
        items = new Item[BodyLoc.sheet.Count];
    }

    private void Start()
    {
        UpdateAnimator();
    }
}
