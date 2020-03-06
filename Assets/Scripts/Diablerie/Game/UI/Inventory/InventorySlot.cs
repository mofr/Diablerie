using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.UI;
using Diablerie.Engine.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diablerie.Game.UI.Inventory
{
    public class InventorySlot : 
        MonoBehaviour,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public int bodyLoc;
        public Item item;
        public Image placeholder;
        public Image highlighter;
        public Image itemImage;
        private bool pointerOver = false;
        private Player _player;

        public void SetPlayer(Player player)
        {
            _player = player;
        }

        private bool CanEquip(Item item)
        {
            return _player.equip.CanEquip(item, bodyLoc);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerOver = true;
            if (_player.HandsItem != null && !CanEquip(_player.HandsItem))
                highlighter.color = Colors.InvItemHighlightForbid;
            else
                highlighter.color = Colors.InvItemHighlight;
            highlighter.gameObject.SetActive(_player.HandsItem != null || item != null);
        }

        private void ShowLabel()
        {
            var rect = Tools.RectTransformToScreenRect(GetComponent<RectTransform>());
            var pos = new Vector2(rect.center.x, rect.yMax);
            Ui.ShowScreenLabel(pos, item.GetDescription());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointerOver = false;
            highlighter.gameObject.SetActive(false);
            Ui.HideScreenLabel();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_player.HandsItem != null && !CanEquip(_player.HandsItem))
            {
                string charClass = _player.charStatInfo.classNameLower;
                AudioManager.instance.Play(charClass + "_impossible_1");
                return;
            }

            Item[] unequipped = _player.equip.Equip(_player.HandsItem, bodyLoc);

            if (_player.HandsItem != null)
                AudioManager.instance.Play(_player.HandsItem.useSound);
            else if (unequipped[0] != null)
                AudioManager.instance.Play(SoundInfo.itemPickup);

            _player.HandsItem = unequipped[0];
            if (unequipped[1] != null)
                if (!_player.inventory.Put(unequipped[1]))
                    Loot.Create(_player.transform.position, unequipped[1]);
        }

        private void OnDisable()
        {
            pointerOver = false;
            highlighter.gameObject.SetActive(false);
            Ui.HideScreenLabel();
        }
    
        private void Update()
        {
            if (!pointerOver)
                return;

            if (item != null && _player.HandsItem == null)
            {
                ShowLabel();
            }
            else
            {
                Ui.HideScreenLabel();
            }
        }
    }
}
