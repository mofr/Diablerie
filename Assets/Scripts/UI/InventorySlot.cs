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

    private void Start()
    {
        highlighter.color = new Color(0.1f, 0.3f, 0.1f, 0.3f);
    }

    private bool Accept(Item item)
    {
        return item.info.type.body && (item.info.type.bodyLoc1 == bodyLoc || item.info.type.bodyLoc2 == bodyLoc);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var mouseItem = PlayerController.instance.mouseItem;
        if (mouseItem != null && !Accept(mouseItem))
            return;
        highlighter.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighter.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var mouseItem = PlayerController.instance.mouseItem;
        if (mouseItem != null && !Accept(mouseItem))
            return;
        item = null;
        Item[] unequipped = PlayerController.instance.equip.Equip(mouseItem, bodyLoc);
        PlayerController.instance.mouseItem = unequipped[0];
        if (unequipped[1] != null)
            Pickup.Create(PlayerController.instance.character.transform.position, unequipped[1]);
    }
}
