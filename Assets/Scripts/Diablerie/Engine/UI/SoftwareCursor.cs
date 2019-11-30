using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Engine.UI
{
    public class SoftwareCursor : MonoBehaviour
    {
        private static SoftwareCursor instance;

        public RawImage image;
        public RectTransform rectTransform;

        private void Awake()
        {
            instance = this;
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            UpdatePosition();
        }

        public static void SetCursor(Texture2D texture)
        {
            SetCursor(texture, Vector2.zero);
        }

        public static void SetCursor(Texture2D texture, Vector2 hotSpot)
        {
            if (texture != null)
            {
                instance.image.texture = texture;
                instance.rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
                instance.rectTransform.pivot = new Vector2(hotSpot.x / texture.width, 1 - hotSpot.y / texture.height);
                instance.UpdatePosition();
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance.gameObject.SetActive(false);
            }
        }

        private void UpdatePosition()
        {
            rectTransform.anchoredPosition = Input.mousePosition;
        }
    }
}
