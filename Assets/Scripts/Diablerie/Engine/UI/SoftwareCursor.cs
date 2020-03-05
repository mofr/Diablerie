using UnityEngine;
using UnityEngine.UI;

namespace Diablerie.Engine.UI
{
    public class SoftwareCursor
    {
        private static SoftwareCursor instance;

        private GameObject _root;
        private RawImage _image;
        private RectTransform _rectTransform;

        public SoftwareCursor(RectTransform parentTransform)
        {
            Debug.Assert(instance == null);
            instance = this;
            _root = new GameObject("SoftwareCursor");
            _rectTransform = _root.AddComponent<RectTransform>();
            _rectTransform.SetParent(parentTransform, false);
            _rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
            _rectTransform.anchorMax = new Vector2(0.0f, 0.0f);
            var behaviour = _root.AddComponent<InternalBehaviour>();
            behaviour.cursor = this;
            _image = _root.AddComponent<RawImage>();
            _image.raycastTarget = false;
        }

        public static void SetCursor(Texture2D texture)
        {
            SetCursor(texture, Vector2.zero);
        }

        public static void SetCursor(Texture2D texture, Vector2 hotSpot)
        {
            if (texture != null)
            {
                instance._image.texture = texture;
                instance._rectTransform.sizeDelta = new Vector2(texture.width, texture.height);
                instance._rectTransform.pivot = new Vector2(hotSpot.x / texture.width, 1 - hotSpot.y / texture.height);
                instance.UpdatePosition();
                instance._root.SetActive(true);
            }
            else
            {
                instance._root.SetActive(false);
            }
        }

        private void UpdatePosition()
        {
            _rectTransform.anchoredPosition = Input.mousePosition;
        }

        private class InternalBehaviour : MonoBehaviour
        {
            public SoftwareCursor cursor;
            
            private void Update()
            {
                cursor.UpdatePosition();
            }
        }
    }
}
