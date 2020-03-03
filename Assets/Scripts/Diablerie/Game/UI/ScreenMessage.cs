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
            GetInstance().ShowInternal(message);
        }

        public static void Hide()
        {
            GetInstance().HideInternal();
        }

        private static ScreenMessage GetInstance()
        {
            if (instance == null)
                instance = new ScreenMessage();
            return instance;
        }

        private ScreenMessage()
        {
            gameObject = new GameObject("Screen Message");
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            var backgroundGameObject = new GameObject("Background");
            var backgroundImage = backgroundGameObject.AddComponent<RawImage>();
            backgroundImage.color = Color.black;
            var backgroundTransform = backgroundGameObject.GetComponent<RectTransform>();
            backgroundTransform.SetParent(gameObject.transform);
            backgroundTransform.anchorMin = new Vector2(0, 0);
            backgroundTransform.anchorMax = new Vector2(1, 1);
            backgroundTransform.pivot = new Vector2(0.5f, 0.5f);
            backgroundTransform.anchoredPosition = new Vector2(0, 0);

            var textGameObject = new GameObject("Text");
            text = textGameObject.AddComponent<Text>();
            var textRectTransform = textGameObject.GetComponent<RectTransform>();
            textRectTransform.SetParent(gameObject.transform);
            textRectTransform.anchorMin = new Vector2(0, 0);
            textRectTransform.anchorMax = new Vector2(1, 1);
            textRectTransform.pivot = new Vector2(0.5f, 0.5f);
            textRectTransform.anchoredPosition = new Vector2(0, 0);
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.font = Fonts.GetFont30();
            Object.DontDestroyOnLoad(gameObject);
        }

        private void ShowInternal(string message)
        {
            text.text = message;
            gameObject.SetActive(true);
        }
        
        private void HideInternal()
        {
            gameObject.SetActive(false);
        }
    }
}
