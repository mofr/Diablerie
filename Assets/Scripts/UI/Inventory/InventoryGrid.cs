using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventoryGrid :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    Inventory _inventory;

    float cellSizeX;
    float cellSizeY;

    RawImage highlighter;
    RectTransform rectTransform;
    bool pointerOver;

    List<Image> itemsImages = new List<Image>();
    List<RawImage> itemsBackgrounds = new List<RawImage>();

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

        highlighter = CreateHighlighter();
        highlighter.gameObject.SetActive(false);

        inventory = PlayerController.instance.inventory;
    }

    RawImage CreateHighlighter()
    {
        GameObject gameObject = new GameObject("highlighter");
        var image = gameObject.AddComponent<RawImage>();
        image.transform.SetParent(transform);
        image.color = new Color(0.1f, 0.3f, 0.1f, 0.3f);
        image.rectTransform.pivot = new Vector2(0, 0);
        image.rectTransform.localScale = new Vector3(1, 1, 1);
        image.rectTransform.localPosition = new Vector3(0, 0, 0);
        image.rectTransform.anchorMin = new Vector2(0, 0);
        image.rectTransform.anchorMax = new Vector2(0, 0);
        return image;
    }

    void OnInventoryUpdate()
    {
        for(int i = 0; i < _inventory.entries.Count; ++i)
        {
            var entry = _inventory.entries[i];
            Image image;
            RawImage bg;
            if (i >= itemsImages.Count)
            {
                bg = CreateHighlighter();
                itemsBackgrounds.Add(bg);
                image = CreateItemImage(entry.item);
                itemsImages.Add(image);
            }
            else
            {
                bg = itemsBackgrounds[i];
                bg.gameObject.SetActive(true);
                image = itemsImages[i];
                image.gameObject.SetActive(true);
            }
            
            image.rectTransform.localPosition = new Vector2(entry.x * cellSizeX, entry.y * cellSizeY);
            image.sprite = entry.item.invSprite;
            image.SetNativeSize();

            bg.color = new Color(0.1f, 0.1f, 0.3f, 0.3f);
            SetRect(bg, entry.x, entry.y, entry.item.info.invWidth, entry.item.info.invHeight);
        }

        for (int i = _inventory.entries.Count; i < itemsImages.Count; ++i)
        {
            itemsImages[i].gameObject.SetActive(false);
            itemsBackgrounds[i].gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var mouseItem = PlayerController.instance.mouseItem;
        var cell = MouseCell();
        if (mouseItem != null)
        {
            Item poppedItem;
            if (_inventory.Put(mouseItem, cell.x, cell.y, out poppedItem))
                PlayerController.instance.mouseItem = poppedItem;
        }
        else
        {
            Item item = _inventory.Take(cell.x, cell.y);
            PlayerController.instance.mouseItem = item;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
        ClearHighlights();
    }

    private void OnDisable()
    {
        pointerOver = false;
        ClearHighlights();
    }

    private void ClearHighlights()
    {
        highlighter.gameObject.SetActive(false);
        UI.HideScreenLabel();
        for (int i = 0; i < _inventory.entries.Count; ++i)
        {
            RawImage bg = itemsBackgrounds[i];
            bg.color = new Color(0.1f, 0.1f, 0.3f, 0.3f);
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

    private void SetRect(Graphic graphic, int x, int y, int width, int height)
    {
        graphic.rectTransform.offsetMin = Vector2.zero;
        graphic.rectTransform.offsetMax = new Vector2(width * cellSizeX, height * cellSizeY);
        graphic.rectTransform.localPosition = new Vector2(x * cellSizeX, y * cellSizeY);
    }

    private void Update()
    {
        if (_inventory == null || !pointerOver)
            return;

        ClearHighlights();

        var cell = MouseCell();
        var mouseItem = PlayerController.instance.mouseItem;
        if (mouseItem != null)
        {
            SetRect(highlighter, cell.x, cell.y, mouseItem.info.invWidth, mouseItem.info.invHeight);
            List<int> coveredEntries;
            highlighter.gameObject.SetActive(_inventory.Fit(mouseItem, cell.x, cell.y, out coveredEntries));
            if (coveredEntries.Count > 1)
            {
                highlighter.color = new Color(0.3f, 0.1f, 0.1f, 0.3f);
            }
            else
            {
                highlighter.color = new Color(0.1f, 0.3f, 0.1f, 0.3f);
                foreach(int entryIndex in coveredEntries)
                {
                    RawImage bg = itemsBackgrounds[entryIndex];
                    bg.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                }
            }
        }
        else
        {
            int entryIndex = _inventory.At(cell.x, cell.y);
            if (entryIndex != -1)
            {
                RawImage bg = itemsBackgrounds[entryIndex];
                bg.color = new Color(0.1f, 0.3f, 0.1f, 0.3f);
                Inventory.Entry entry = _inventory.entries[entryIndex];
                UI.ShowScreenLabel(Input.mousePosition, entry.item.GetDescription());
            }
        }
    }
}