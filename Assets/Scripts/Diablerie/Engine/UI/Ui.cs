using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Engine.UI
{
    public class Ui : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static Ui instance;

        private RectTransform rectTransform;
        private ScreenLabel screenLabel;
        private SoftwareCursor _softwareCursor;
        private GameObject currentHover;

        void Awake()
        {
            instance = this;
            rectTransform = transform as RectTransform;
            screenLabel = new ScreenLabel(rectTransform);
            screenLabel.Hide();
            _softwareCursor = new SoftwareCursor(rectTransform);
        }
        
        public void OnPointerEnter(PointerEventData eventData) {
            if (eventData.pointerCurrentRaycast.gameObject != null) {
                currentHover = eventData.pointerCurrentRaycast.gameObject;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            currentHover = null;
        }
        
        public static bool Hover => instance.currentHover != null;

        public RectTransform RectTransform => rectTransform;

        public static void ShowScreenLabel(Vector2 position, string text)
        {
            instance.screenLabel.Show(position, text);
        }

        public static void HideScreenLabel()
        {
            instance.screenLabel.Hide();
        }
    }
}
