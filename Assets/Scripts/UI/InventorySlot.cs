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

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlighter.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlighter.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var mouseItem = PlayerController.instance.mouseItem;
        PlayerController.instance.mouseItem = item;
        item = null;
        PlayerController.instance.equip.Equip(mouseItem, bodyLoc);
    }
}
