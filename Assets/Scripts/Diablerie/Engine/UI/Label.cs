using System;
using Diablerie.Engine.Utility;
using Diablerie.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Engine.UI
{
    public class Label
    {
        private static readonly Color BackgroundColor = new Color(0, 0, 0, 0.95f); 
        private static readonly Color HighlightedBackgroundColor = new Color(0.05f, 0.15f, 0.4f, 0.95f);
        
        private GameObject root;
        private RectTransform rectTransform;
        private Text text;
        private bool highlighed;
        private Image backgroundImage;

        public Label(Transform parentTransform)
        {
            root = new GameObject("Label");
            rectTransform = root.AddComponent<RectTransform>();
            rectTransform.SetParent(parentTransform, false);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.localScale = new Vector3(1 / Iso.pixelsPerUnit, 1 / Iso.pixelsPerUnit);
            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingLayerID = SortingLayers.WorldUI;
            root.AddComponent<GraphicRaycaster>();
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
            backgroundImage = root.AddComponent<Image>();
            backgroundImage.color = BackgroundColor;
            backgroundImage.raycastTarget = true;
        }

        public void Show(Vector2 position, string text, bool fitIntoTheScreen = true)
        {
            if (!root.activeSelf || rectTransform.anchoredPosition != position || this.text.text != text)
            {
                this.text.text = text;
                rectTransform.anchoredPosition = position;
                root.SetActive(true);
                Canvas.ForceUpdateCanvases();
                if (fitIntoTheScreen)
                    FitIntoTheScreen();
            }
        }

        public void Hide()
        {
            root.SetActive(false);
        }
        
        private void FitIntoTheScreen()
        {
            var rect = GetScreenRect();
            Vector2 screenSize = Ui.instance.RectTransform.sizeDelta;
            Vector2 minPosition = Camera.main.WorldToViewportPoint(rect.min);
            Vector2 maxPosition = Camera.main.WorldToViewportPoint(rect.max);
            float leftOverflow = Math.Min(minPosition.x, 0);
            float rightOverflow = Math.Max(maxPosition.x - 1, 0);
            float topOverflow = Math.Max(maxPosition.y - 1, 0);
            float horizontalOffset = -(leftOverflow + rightOverflow) * screenSize.x / Iso.pixelsPerUnit;
            float verticalOffset = -topOverflow * screenSize.y / Iso.pixelsPerUnit;
            rectTransform.anchoredPosition += new Vector2(horizontalOffset, verticalOffset);
        }
        
        public Rect GetScreenRect()
        {
            var rect = new Rect(rectTransform.anchoredPosition, rectTransform.rect.size / Iso.pixelsPerUnit);
            rect.x -= rect.width / 2;
            return rect;
        }

        public GameObject gameObject => root;

        public bool Highlighed
        {
            get => highlighed;
            set
            {
                if (value != highlighed)
                {
                    highlighed = value;
                    backgroundImage.color = highlighed ? HighlightedBackgroundColor : BackgroundColor;
                }
            }
        }

        public RectTransform RectTransform => rectTransform;
    }
}
