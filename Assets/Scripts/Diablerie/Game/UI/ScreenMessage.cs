using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Game.UI
{
    public class ScreenMessage : MonoBehaviour
    {
        public static void Show(string message)
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
            text.text = message;
            text.color = Color.white;
            text.font = font;
        }
    }
}
