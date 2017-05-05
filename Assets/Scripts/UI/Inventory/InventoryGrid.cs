using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventoryGrid :
    MonoBehaviour,
    IPointerDownHandler
{
    Inventory _inventory;

    float cellSizeX;
    float cellSizeY;

    RawImage highlighter;
    RectTransform rectTransform;

    List<Image> itemsImages = new List<Image>();

    public Inventory inventory
    {
        get { return _inventory; }
        set
        {
            if (_inventory == value)
                return;

            if (_inventory != null)
                _inventory.OnUpdate -= OnInventoryUpdate;

            _inventory = value;
            if (_inventory != null)
                _inventory.OnUpdate += OnInventoryUpdate;

            cellSizeX = rectTransform.rect.width / _inventory.sizeX;
            cellSizeY = rectTransform.rect.height / _inventory.sizeY;
            OnInventoryUpdate();
        }
    }
    
    private Image CreateItemImage(Item item)
    {
        GameObject gameObject = new GameObject();
        gameObject.name = item.info.name;
        var image = gameObject.AddComponent<Image>();
        image.transform.SetParent(transform);
        image.rectTransform.pivot = new Vector2(0, 0);
        image.rectTransform.localScale = new Vector3(1, 1, 1);
        image.rectTransform.localPosition = new Vector3(0, 0, 0);
        image.rectTransform.anchorMin = new Vector2(0, 0);
        image.rectTransform.anchorMax = new Vector2(0, 0);
        return image;
    }

    private void Awake()
    {
        rectTransform = transform as RectTransform;

        GameObject highlighterObject = new GameObject("highlighter");
        highlighter = highlighterObject.AddComponent<RawImage>();
        highlighter.transform.SetParent(transform);
        highlighter.color = new Color(0.1f, 0.3f, 0.1f, 0.3f);
        highlighter.rectTransform.pivot = new Vector2(0, 0);
        highlighter.rectTransform.localScale = new Vector3(1, 1, 1);
        highlighter.rectTransform.localPosition = new Vector3(0, 0, 0);
        highlighter.rectTransform.anchorMin = new Vector2(0, 0);
        highlighter.rectTransform.anchorMax = new Vector2(0, 0);

        inventory = PlayerController.instance.inventory;
    }

    void OnInventoryUpdate()
    {
        for(int i = 0; i < _inventory.entries.Count; ++i)
        {
            var entry = _inventory.entries[i];
            Image image;
            if (i >= itemsImages.Count)
            {
                image = CreateItemImage(entry.item);
                itemsImages.Add(image);
            }
            else
            {
                image = itemsImages[i];
                image.gameObject.SetActive(true);
            }
            
            image.rectTransform.localPosition = new Vector2(entry.x * cellSizeX, entry.y * cellSizeY);
            image.sprite = entry.item.invSprite;
            image.SetNativeSize();
        }

        for (int i = _inventory.entries.Count; i < itemsImages.Count; ++i)
        {
            itemsImages[i].gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var mouseItem = PlayerController.instance.mouseItem;
        var cell = MouseCell();
        if (mouseItem != null)
        {
            if (_inventory.Put(mouseItem, cell.x, cell.y))
                PlayerController.instance.mouseItem = null;
        }
        else
        {
            Item item = _inventory.Take(cell.x, cell.y);
            PlayerController.instance.mouseItem = item;
        }
    }

    private Vector2i MouseCell()
    {
        var mouseItem = PlayerController.instance.mouseItem;
        Vector3 pointerPosition = Input.mousePosition;
        if (mouseItem != null)
        {
            pointerPosition -= new Vector3(mouseItem.info.invWidth * cellSizeX, mouseItem.info.invHeight * cellSizeY) / 2;
            pointerPosition += new Vector3(cellSizeX, cellSizeY) * 0.5f; // adjust to center of cell
        }
        return GetPointerCell(pointerPosition);
    }

    private Vector2i GetPointerCell(Vector3 pointerPosition)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerPosition, Camera.main, out pos);
        int cellX = Mathf.FloorToInt(pos.x / cellSizeX);
        int cellY = Mathf.FloorToInt(pos.y / cellSizeY);
        return new Vector2i(cellX, cellY);
    }

    private void Highlight(int x, int y, int width, int height)
    {
        highlighter.rectTransform.offsetMin = Vector2.zero;
        highlighter.rectTransform.offsetMax = new Vector2(width * cellSizeX, height * cellSizeY);
        highlighter.rectTransform.localPosition = new Vector2(x * cellSizeX, y * cellSizeY);
    }

    private void Update()
    {
        if (_inventory == null)
            return;
        
        var cell = MouseCell();
        var mouseItem = PlayerController.instance.mouseItem;
        if (mouseItem != null)
        {
            Highlight(cell.x, cell.y, mouseItem.info.invWidth, mouseItem.info.invHeight);
            highlighter.gameObject.SetActive(_inventory.Fits(mouseItem, cell.x, cell.y));
        }
        else
        {
            Inventory.Entry entry = _inventory.At(cell.x, cell.y);
            if (entry.item != null)
                Highlight(entry.x, entry.y, entry.item.info.invWidth, entry.item.info.invHeight);
            highlighter.gameObject.SetActive(entry.item != null);
        }
    }
}