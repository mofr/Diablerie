using Diablerie.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Engine.UI
{
    public class ScreenLabel
    {
        private GameObject root;
        private Text text;
        private RectTransform rectTransform;
        
        public ScreenLabel(Transform parent)
        {
            root = new GameObject("ScreenLabel");
            rectTransform = root.AddComponent<RectTransform>();
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            var contentFitter = root.AddComponent<ContentSizeFitter>();
            contentFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var layoutGroup = root.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(6, 6, 0, 4);
            layoutGroup.spacing = 0;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = true;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            var textGameObject = new GameObject("Text");
            textGameObject.transform.SetParent(rectTransform, false);
            text = textGameObject.AddComponent<Text>();
            text.alignment = TextAnchor.MiddleCenter;
            text.alignByGeometry = true;
            text.font = Fonts.GetFont16();
            text.supportRichText = true;
            text.raycastTarget = false;
            var backgroundImage = root.AddComponent<Image>();
            backgroundImage.color = new Color(0, 0, 0, 224);
            backgroundImage.raycastTarget = false;
        }

        public bool Enabled
        {
            get => root.activeSelf;
            set => root.SetActive(value);
        }

        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        public void SetPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }
    }
}