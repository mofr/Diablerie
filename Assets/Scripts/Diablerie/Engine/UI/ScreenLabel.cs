using System;
using Diablerie.Engine.Utility;
using Diablerie.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Engine.UI
{
    public class ScreenLabel
    {
        private GameObject root;
        private RectTransform rectTransform;
        private RectTransform parentTransform;
        private Text text;

        public ScreenLabel(RectTransform parentTransform)
        {
            this.parentTransform = parentTransform;
            root = new GameObject("ScreenLabel");
            rectTransform = root.AddComponent<RectTransform>();
            rectTransform.SetParent(this.parentTransform, false);
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
            backgroundImage.color = new Color(0, 0, 0, 0.95f);
            backgroundImage.raycastTarget = false;
        }

        public void Show(Vector2 position, string text)
        {
            if (!root.activeSelf || rectTransform.anchoredPosition != position || this.text.text != text)
            {
                this.text.text = text;
                rectTransform.anchoredPosition = position;
                root.SetActive(true);
                FitIntoParent();
            }
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        private void FitIntoParent()
        {
            Canvas.ForceUpdateCanvases();
            Rect rect = Tools.RectTransformToScreenRect(rectTransform);
            Vector2 parentSize = parentTransform.rect.size;
            float leftOverflow = -Math.Min(rect.xMin, 0);
            float rightOverflow = Math.Max(rect.xMax - parentSize.x, 0);
            float topOverflow = Math.Max(rect.yMax - parentSize.y, 0);
            rectTransform.anchoredPosition -= new Vector2(leftOverflow + rightOverflow, topOverflow);
        }
    }
}