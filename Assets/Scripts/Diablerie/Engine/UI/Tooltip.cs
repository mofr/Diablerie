using Diablerie.Engine.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Engine.UI
{
    public class Tooltip : 
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public string text;

        private void ShowLabel()
        {
            var rect = Tools.RectTransformToScreenRect(GetComponent<RectTransform>());
            var pos = new Vector2(rect.center.x, rect.yMax);
            Ui.ShowScreenLabel(pos, text);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowLabel();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Ui.HideScreenLabel();
        }

        void OnDisable()
        {
            Ui.HideScreenLabel();
        }
    }
}
