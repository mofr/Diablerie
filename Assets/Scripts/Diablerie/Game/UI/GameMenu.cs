using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class GameMenu
    {
        private static GameMenu instance;
        private GameObject gameObject;
        private Font font;
        
        public static void Show()
        {
            GetInstance().ShowInternal();
        }

        public static void Hide()
        {
            GetInstance().HideInternal();
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
            gameObject.AddComponent<InternalBehaviour>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            font = Fonts.GetFont42();
            AddMenuItem("EXIT GAME");
        }
        
        private void ShowInternal()
        {
            gameObject.SetActive(true);
        }

        private void HideInternal()
        {
            gameObject.SetActive(false);
        }

        private void AddMenuItem(string itemName)
        {
            var textGameObject = new GameObject("MENU");
            var text = textGameObject.AddComponent<Text>();
            var textRectTransform = textGameObject.GetComponent<RectTransform>();
            textRectTransform.SetParent(gameObject.transform);
            textRectTransform.anchorMin = new Vector2(0, 0);
            textRectTransform.anchorMax = new Vector2(1, 1);
            textRectTransform.pivot = new Vector2(0.5f, 0.5f);
            textRectTransform.anchoredPosition = new Vector2(0, 0);
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.font = font;
            text.text = itemName;
        }

        private class InternalBehaviour : MonoBehaviour
        {
            void Update()
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                    Hide();
            }
        }
    }
}
