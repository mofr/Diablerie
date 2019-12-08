using System.Collections.Generic;
using Diablerie.Engine;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class GameMenu
    {
        private static GameMenu instance;
        private GraphicRaycaster raycaster;
        private GameObject root;
        private RectTransform layoutGroupTransform;
        private GameObject leftStar;
        private GameObject rightStar;
        private List<MenuItem> items = new List<MenuItem>();
        private int selectedIndex = 0;
        private int itemHeight = 50;

        public static void Show()
        {
            GetInstance().ShowInternal();
        }

        public static void Hide()
        {
            GetInstance().HideInternal();
        }

        public static bool IsVisible()
        {
            return GetInstance().root.activeSelf;
        }

        private static GameMenu GetInstance()
        {
            if (instance == null)
                instance = new GameMenu();
            return instance;
        }

        private GameMenu()
        {
            root = new GameObject("Game Menu");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "UI";
            raycaster = root.AddComponent<GraphicRaycaster>();
            var behaviour = root.AddComponent<InternalBehaviour>();
            behaviour.menu = this;
            var layoutGroupObject = new GameObject("Vertical Layout");
            layoutGroupObject.transform.SetParent(root.transform, false);
            var layoutGroup = layoutGroupObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 0;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            layoutGroup.childControlHeight = true;
            layoutGroupTransform = layoutGroupObject.GetComponent<RectTransform>();
            layoutGroupTransform.anchorMin = new Vector2(0, 0.5f);
            layoutGroupTransform.anchorMax = new Vector2(1, 0.5f);
            layoutGroupTransform.pivot = new Vector2(0.5f, 0.5f);
            layoutGroupTransform.anchoredPosition = new Vector2(0, 0);
            AddMenuItem("OPTIONS", enabled: false);
            AddMenuItem("EXIT GAME", GameManager.QuitGame);
            AddMenuItem("RETURN TO GAME", HideInternal);
            HideInternal();
            leftStar = CreateStar(true);
            rightStar = CreateStar(false);
            selectedIndex = 1;
        }
        
        private void ShowInternal()
        {
            root.SetActive(true);
        }

        private void HideInternal()
        {
            root.SetActive(false);
        }

        private void AddMenuItem(string itemName, UnityAction action = null, bool enabled = true)
        {
            var menuItem = new MenuItem(itemName, enabled);
            menuItem.rectTransform.SetParent(layoutGroupTransform, false);
            menuItem.action = action;
            items.Add(menuItem);
            layoutGroupTransform.sizeDelta = new Vector2(0, itemHeight * items.Count);
        }

        private GameObject CreateStar(bool left)
        {
            Palette.LoadPalette(0);
            var spritesheet = Spritesheet.Load(@"data\global\ui\CURSOR\pentspin");
            var sprites = spritesheet.GetSprites(0);
            float width = sprites[0].rect.width;
            float height = sprites[0].rect.height;
            var star = new GameObject("star");
            star.transform.localScale = new Vector3(Iso.pixelsPerUnit, Iso.pixelsPerUnit);
            star.transform.localPosition = new Vector3((left ? -250 : 250) - width / 2, -height / 2); // TODO remove hardcoded offset
            var animator = star.AddComponent<SpriteAnimator>();
            animator.sprites = sprites;
            animator.fps = 20;
            animator.reversed = left;
            animator.OffsetTime(left ? 0.1f : 0);
            var spriteRenderer = star.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "UI";
            return star;
        }

        private class InternalBehaviour : MonoBehaviour
        {
            public GameMenu menu;
            
            void Update()
            {
                UpdateSelectedItem();
                UpdateStarsPositions();
                if (Input.GetKeyDown(KeyCode.Escape))
                    Hide();
            }

            private void UpdateSelectedItem()
            {
                List<RaycastResult> results = new List<RaycastResult>();
                var pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.mousePosition;
                menu.raycaster.Raycast(pointerEventData, results);
            }

            private void UpdateStarsPositions()
            {
                MenuItem selectedItem = menu.items[menu.selectedIndex];
                menu.leftStar.transform.SetParent(selectedItem.gameObject.transform, false);
                menu.rightStar.transform.SetParent(selectedItem.gameObject.transform, false);
            }
        }

        private class MenuItem
        {
            public GameObject gameObject;
            public RectTransform rectTransform;
            public UnityAction action;
            
            public MenuItem(string name, bool enabled)
            {
                gameObject = new GameObject(name);
                var text = gameObject.AddComponent<Text>();
                rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                text.alignment = TextAnchor.LowerCenter;
                text.color = enabled ? Color.white : Color.grey;
                text.font = Fonts.GetFont42();
                text.text = name;
                
                if (enabled)
                {
                    var button = gameObject.AddComponent<Button>();
                    button.transition = Selectable.Transition.None;
                    button.onClick.AddListener(OnClick);
                }
            }

            private void OnClick()
            {
                AudioManager.instance.Play("cursor_select");
                if (action != null)
                    action();
            }
        }
    }
}
