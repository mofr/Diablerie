using Diablerie.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Engine.UI
{
    public class Label
    {
        private GameObject root;
        private RectTransform rectTransform;
        private Text text;

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
            canvas.sortingLayerName = "UI";
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
            }
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}
