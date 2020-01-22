using Diablerie.Engine.Datasheets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Diablerie.Engine.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public delegate void OnClickHandler();

        public event OnClickHandler OnClick;
        
        public Sprite normalState;
        public Sprite pressedState;
        public Sprite disabledState;
        
        private Image _image;
        private RectTransform _textPosition;
        private bool _disabled;

        public bool Disabled
        {
            get => _disabled;

            set
            {
                _disabled = value;
                OnDisabledChanged();
            }
        }
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.sprite = _disabled ? disabledState : normalState;
            _textPosition = GetComponent<Transform>().GetChild(0).GetComponent<RectTransform>();
        }

        private void OnDisabledChanged()
        {
            if (_image == null)
            {
                return;
            }
            
            _image.sprite = _disabled ? disabledState : normalState;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_disabled)
            {
                _image.sprite = pressedState;
                _textPosition.Translate(-2, -2, 0);
                AudioManager.instance.Play(SoundInfo.cursorButtonClick);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_disabled)
            {
                _image.sprite = normalState;
                _textPosition.Translate(2, 2, 0);
                OnClick?.Invoke();
            }
        }
    }
}