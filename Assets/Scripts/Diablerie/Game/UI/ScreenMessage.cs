using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class ScreenMessage
    {
        private static ScreenMessage instance;
        private GameObject gameObject;
        private Text text;
        
        public static void Show(string message)
        {
            if (instance == null)
                instance = Create();
            instance.text.text = message;
            instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            if (instance == null)
                return;
            instance.gameObject.SetActive(false);
        }

        private static ScreenMessage Create()
        {
            var root = new GameObject("Canvas");
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var font = Resources.Load<Font>("Fonts/font30");

            var textGameObject = new GameObject("Text");
            var text = textGameObject.AddComponent<Text>();
            var textRectTransform = textGameObject.GetComponent<RectTransform>();
            textRectTransform.SetParent(root.transform);
            textRectTransform.anchorMin = new Vector2(0, 0);
            textRectTransform.anchorMax = new Vector2(1, 1);
            textRectTransform.pivot = new Vector2(0.5f, 0.5f);
            textRectTransform.anchoredPosition = new Vector2(0, 0);
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.font = font;
            
            instance = new ScreenMessage();
            instance.gameObject = root;
            instance.text = text;
            return instance;
        }
    }
}
