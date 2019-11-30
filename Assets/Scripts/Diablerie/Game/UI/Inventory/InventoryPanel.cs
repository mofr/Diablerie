using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI.Inventory
{
    public class InventoryPanel : MonoBehaviour
    {
        static public InventoryPanel instance;

        public GameObject panel;
        public Text goldText;

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
        InventorySlot[] slots;

        void Awake()
        {
            instance = this;
            slots = new InventorySlot[BodyLoc.sheet.Count];
            for (int i = 0; i < slots.Length; ++i)
            {
                Transform slotTransform = panel.transform.Find(BodyLoc.sheet[i].code);
                var slot = slotTransform.gameObject.AddComponent<InventorySlot>();
                slot.placeholder = slotTransform.Find("placeholder").GetComponent<Image>();
                slot.highlighter = slotTransform.Find("highlighter").GetComponent<Image>();
                slot.itemImage = slotTransform.Find("item").GetComponent<Image>();
                slot.bodyLoc = i;
                slots[i] = slot;
            }
        }

        public void Update()
        {
            goldText.text = PlayerController.instance.inventory.gold.ToString();
        }

        public void OnGoldButton()
        {
            if (PlayerController.instance.inventory.gold > 0)
            {
                var item = Item.Create("gld");
                item.quantity = PlayerController.instance.inventory.gold;
                Pickup.Create(PlayerController.instance.character.transform.position, item);
                PlayerController.instance.inventory.gold = 0;
            }
        }

        public void ToggleVisibility()
        {
            visible ^= true;
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
                Item item = null;
                if (_equip != null)
                    item = _equip.items[i];

                if (item != null)
                {
                    slots[i].itemImage.sprite = item.invSprite;
                    slots[i].itemImage.SetNativeSize();
                }

                slots[i].itemImage.gameObject.SetActive(item != null);
                slots[i].placeholder.color = new Color(1, 1, 1, item == null ? 1 : 0);
                slots[i].item = item;
            }
        }

        private void OnDisable()
        {
            Ui.HideScreenLabel();
        }
    }
}
