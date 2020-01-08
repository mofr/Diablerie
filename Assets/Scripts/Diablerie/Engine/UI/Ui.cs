using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Engine.UI
{
    public class Ui : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static Ui instance;
    
        public SoftwareCursor softwareCursorPrefab;
        
        private ScreenLabel screenLabel;
        private GameObject currentHover;

        void Awake()
        {
            instance = this;
            screenLabel = new ScreenLabel(transform as RectTransform);
            screenLabel.Hide();
            Instantiate(instance.softwareCursorPrefab, transform);
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
