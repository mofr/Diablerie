using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private bool CanAccept(Item item)
    {
        return item.info.type.body && (item.info.type.bodyLoc1 == bodyLoc || item.info.type.bodyLoc2 == bodyLoc);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var mouseItem = PlayerController.instance.mouseItem;
        if (mouseItem != null && !CanAccept(mouseItem))
            highlighter.color = new Color(0.3f, 0.1f, 0.1f, 0.3f);
        else
            highlighter.color = new Color(0.1f, 0.3f, 0.1f, 0.3f);
        highlighter.gameObject.SetActive(mouseItem != null || item != null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighter.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var mouseItem = PlayerController.instance.mouseItem;
        if (mouseItem != null && !CanAccept(mouseItem))
            return;
        item = null;
        Item[] unequipped = PlayerController.instance.equip.Equip(mouseItem, bodyLoc);
        PlayerController.instance.mouseItem = unequipped[0];
        if (unequipped[1] != null)
            if (!PlayerController.instance.inventory.Put(unequipped[1]))
                Pickup.Create(PlayerController.instance.character.transform.position, unequipped[1]);
    }
}
