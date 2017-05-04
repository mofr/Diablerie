using UnityEngine;

public class Equipment : MonoBehaviour
{
    Character character;
    COFAnimator animator;
    public Item[] items;

    public delegate void OnUpdateHandler();
    public event OnUpdateHandler OnUpdate;

    static string[] armorTypes = new string[] { "LIT", "MED", "HVY" };
    static string[] defaultEquip = new string[] { "LIT", "LIT", "LIT", "LIT", "LIT", "", "", "", "LIT", "LIT", "", "", "", "", "", "" };

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
        
        UpdateAnimator();

        if (OnUpdate != null)
            OnUpdate();

        return previous;
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
                    character.weaponClass = item.info.weapon.wClass;
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
