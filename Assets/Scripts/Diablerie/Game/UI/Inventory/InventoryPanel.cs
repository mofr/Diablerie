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
        public static InventoryPanel instance;

        public GameObject panel;
        public Text goldText;

        public void SetPlayer(Player player)
        {
            if (_player == player)
                return;

            if (_player != null)
                _player.equip.OnUpdate -= UpdateEquip;

            _player = player;
            if (_player != null)
                _player.equip.OnUpdate += UpdateEquip;

            foreach (var slot in slots)
            {
                slot.SetPlayer(player);
            }

            UpdateEquip();
        }

        private Player _player;
        private InventorySlot[] slots;

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
                slot.SetPlayer(_player);
                slots[i] = slot;
            }
        }

        public void Update()
        {
            goldText.text = _player.inventory.gold.ToString();
        }

        public void OnGoldButton()
        {
            if (_player.inventory.gold > 0)
            {
                var item = Item.Create("gld");
                item.quantity = _player.inventory.gold;
                Pickup.Create(_player.transform.position, item);
                _player.inventory.gold = 0;
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

        private void UpdateEquip()
        {
            for (int i = 0; i < slots.Length; ++i)
            {
                Item item = null;
                if (_player.equip != null)
                    item = _player.equip.items[i];

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
