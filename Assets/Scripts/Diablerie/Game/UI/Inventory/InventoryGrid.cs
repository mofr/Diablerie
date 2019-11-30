using System.Collections.Generic;
using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.UI;
using Diablerie.Engine.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diablerie.Game.UI.Inventory
{
    public class InventoryGrid :
        MonoBehaviour,
        IPointerDownHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        Engine.Inventory _inventory;

        float cellSizeX;
        float cellSizeY;

        RawImage highlighter;
        RectTransform rectTransform;
        bool pointerOver;

        List<Image> itemsImages = new List<Image>();
        List<RawImage> itemsBackgrounds = new List<RawImage>();

        public Engine.Inventory inventory
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
            image.color = Colors.InvItemHighlight;
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

                bg.color = Colors.InvItemBackground;
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
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (mouseItem != null)
                {
                    Item poppedItem;
                    if (_inventory.Put(mouseItem, cell.x, cell.y, out poppedItem))
                    {
                        AudioManager.instance.Play(mouseItem.dropSound);
                        PlayerController.instance.mouseItem = poppedItem;
                    }
                }
                else
                {
                    Item item = _inventory.Take(cell.x, cell.y);
                    if (item != null)
                    {
                        PlayerController.instance.mouseItem = item;
                        AudioManager.instance.Play(SoundInfo.itemPickup);
                    }
                }
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                Item item = _inventory.ItemAt(cell.x, cell.y);
                if (item != null)
                {
                    if (!item.identified)
                    {
                        AudioManager.instance.Play("cursor_identify_item");
                        item.identified = true;
                        OnInventoryUpdate();
                    }
                    else if (item.info.misc != null && item.info.misc.useable)
                    {
                        AudioManager.instance.Play(item.useSound);
                        _inventory.Take(cell.x, cell.y);
                        PlayerController.instance.Use(item.info.misc);
                    }
                }
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
            Ui.HideScreenLabel();
            for (int i = 0; i < _inventory.entries.Count; ++i)
            {
                RawImage bg = itemsBackgrounds[i];
                bg.color = Colors.InvItemBackground;
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
                    highlighter.color = Colors.InvItemHighlightForbid;
                }
                else
                {
                    highlighter.color = Colors.InvItemHighlight;
                    foreach(int entryIndex in coveredEntries)
                    {
                        RawImage bg = itemsBackgrounds[entryIndex];
                        bg.color = Colors.InvItemHighlightSwap;
                    }
                }
            }
            else
            {
                int entryIndex = _inventory.At(cell.x, cell.y);
                if (entryIndex != -1)
                {
                    RawImage bg = itemsBackgrounds[entryIndex];
                    bg.color = Colors.InvItemHighlight;
                    Engine.Inventory.Entry entry = _inventory.entries[entryIndex];
                    Ui.ShowScreenLabel(Input.mousePosition, entry.item.GetDescription());
                }
            }
        }
    }
}