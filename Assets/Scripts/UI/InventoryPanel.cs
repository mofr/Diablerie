using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    static public InventoryPanel instance;

    public GameObject panel;

    public Equipment equip
    {
        get { return _equip; }
        set
        {
            if (_equip == value)
                return;

            if (_equip != null)
                _equip.OnUpdate -= UpdateEquip;

            _equip = value;
            if (_equip != null)
                _equip.OnUpdate += UpdateEquip;

            UpdateEquip();
        }
    }

    Equipment _equip;
    Image[] slots;
    Sprite[] placeholders;

    void Awake()
    {
        instance = this;
        slots = new Image[BodyLoc.sheet.Count];
        placeholders = new Sprite[BodyLoc.sheet.Count];
        for (int i = 0; i < slots.Length; ++i)
        {
            Transform slotTransform = panel.transform.Find(BodyLoc.sheet[i].code);
            slots[i] = slotTransform.GetComponent<Image>();
            placeholders[i] = slots[i].sprite;
        }
    }

    public bool visible
    {
        set { panel.SetActive(value); }
        get { return panel.activeSelf; }
    }

    void UpdateEquip()
    {
        for (int i = 0; i < slots.Length; ++i)
        {
            Sprite sprite = placeholders[i];
            if (_equip != null)
            {
                Item item = _equip.items[i];
                if (item != null)
                {
                    var dc6 = DC6.Load(item.info.invFile);
                    sprite = dc6.GetSprites(0)[0];
                }
            }
            slots[i].sprite = sprite;
            slots[i].SetNativeSize();
        }
    }
}
