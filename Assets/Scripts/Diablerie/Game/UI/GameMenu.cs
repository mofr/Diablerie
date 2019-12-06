using System.Collections.Generic;
using Diablerie.Engine;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class GameMenu
    {
        private static GameMenu instance;
        private GameObject gameObject;
        private GameObject leftStar;
        private GameObject rightStar;
        private List<MenuItem> items = new List<MenuItem>();

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
            return GetInstance().gameObject.activeSelf;
        }

        private static GameMenu GetInstance()
        {
            if (instance == null)
                instance = new GameMenu();
            return instance;
        }

        private GameMenu()
        {
            gameObject = new GameObject("Game Menu");
            var canvas = gameObject.AddComponent<Canvas>();
            var behaviour = gameObject.AddComponent<InternalBehaviour>();
            behaviour.menu = this;
            var layoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 60;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.MiddleCenter;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            AddMenuItem("OPTIONS", enabled: false);
            AddMenuItem("EXIT GAME");
            AddMenuItem("RETURN TO GAME");
            HideInternal();
            leftStar = CreateStar(true);
            rightStar = CreateStar(false);
        }
        
        private void ShowInternal()
        {
            gameObject.SetActive(true);
        }

        private void HideInternal()
        {
            gameObject.SetActive(false);
        }

        private void AddMenuItem(string itemName, bool enabled = true)
        {
            var menuItem = new MenuItem(itemName, enabled);
            menuItem.rectTransform.SetParent(gameObject.transform);
            items.Add(menuItem);
        }

        private GameObject CreateStar(bool reversed)
        {
            Palette.LoadPalette(0);
            var spritesheet = Spritesheet.Load(@"data\global\ui\CURSOR\pentspin");
            var star = new GameObject("star");
            var animator = star.AddComponent<SpriteAnimator>();
            animator.sprites = spritesheet.GetSprites(0);
            animator.fps = 20;
            animator.reversed = reversed;
            animator.OffsetTime(Random.Range(0, 2));
            return star;
        }

        private class InternalBehaviour : MonoBehaviour
        {
            public GameMenu menu;
            
            void Update()
            {
                int selectedIndex = 0;
                MenuItem selectedItem = menu.items[selectedIndex];
                var position = Camera.main.ScreenToWorldPoint(selectedItem.rectTransform.position);
                position.z = 0;
                menu.leftStar.transform.position = position;
                if (Input.GetKeyDown(KeyCode.Escape))
                    Hide();
            }
        }

        private class MenuItem
        {
            public GameObject gameObject;
            public RectTransform rectTransform;
            
            public MenuItem(string name, bool enabled)
            {
                gameObject = new GameObject(name);
                var text = gameObject.AddComponent<Text>();
                rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(0, 0);
                text.alignment = TextAnchor.MiddleCenter;
                text.color = enabled ? Color.white : Color.grey;
                text.font = Fonts.GetFont42();
                text.text = name;
            }
        }
    }
}
