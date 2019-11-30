using Diablerie.Engine.Datasheets;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Engine.UI
{
    public class ButtonSound : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            AudioManager.instance.Play(SoundInfo.cursorButtonClick);
        }
    }
}
