using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.UI;
using Diablerie.Engine.Utility;
using Diablerie.Engine.World;
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
        private Player player;

        private bool CanEquip(Item item)
        {
            return player.equip.CanEquip(item, bodyLoc);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointerOver = true;
            if (player.HandsItem != null && !CanEquip(player.HandsItem))
                highlighter.color = Colors.InvItemHighlightForbid;
            else
                highlighter.color = Colors.InvItemHighlight;
            highlighter.gameObject.SetActive(player.HandsItem != null || item != null);
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
            if (player.HandsItem != null && !CanEquip(player.HandsItem))
            {
                string charClass = player.charStat.info.classNameLower;
                AudioManager.instance.Play(charClass + "_impossible_1");
                return;
            }

            Item[] unequipped = player.equip.Equip(player.HandsItem, bodyLoc);

            if (player.HandsItem != null)
                AudioManager.instance.Play(player.HandsItem.useSound);
            else if (unequipped[0] != null)
                AudioManager.instance.Play(SoundInfo.itemPickup);

            player.HandsItem = unequipped[0];
            if (unequipped[1] != null)
                if (!player.inventory.Put(unequipped[1]))
                    Pickup.Create(player.transform.position, unequipped[1]);
        }

        private void OnEnable()
        {
            player = WorldState.instance.Player;  // TODO pass to the constructor (get rid of prefabs first)
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

            if (item != null && player.HandsItem == null)
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
