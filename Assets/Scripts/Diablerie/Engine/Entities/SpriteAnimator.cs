using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        public bool loop = true;
        public bool hideOnFinish = false;
        public float fps = 25;
        public bool reversed = false;
        public bool useUnscaledTime = false;
        public event System.Action OnFinish;

        private SpriteRenderer _renderer;
        private Sprite[] _sprites;
        private float _time = 0;
        private int _frameIndex = 0;
        private bool _finished = false;

        private int _triggerFrame = -1;
        private System.Action _triggerAction;

        public SpriteRenderer Renderer => _renderer;

        public void SetSprites(Sprite[] sprites)
        {
            _sprites = sprites;
        }

        public void Restart()
        {
            _time = 0;
            _frameIndex = 0;
            _finished = false;
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.material = Materials.normal;
        }

        public void SetTrigger(int frame, System.Action action)
        {
            _triggerFrame = frame;
            _triggerAction = action;
        }

        public void OffsetTime(float offset)
        {
            _time += offset;
        }

        private void Update()
        {
            if (_sprites == null || _finished)
            {
                return;
            }

            var newFrameIndex = (int)(_time * fps);
            if (newFrameIndex >= _sprites.Length)
            {
                if (loop)
                {
                    newFrameIndex %= _sprites.Length;
                }
                else
                {
                    newFrameIndex = _sprites.Length - 1;
                    _finished = true;
                }
            }

            if (_frameIndex != newFrameIndex)
            {
                if (_frameIndex < _triggerFrame && newFrameIndex >= _triggerFrame)
                {
                    _triggerAction();
                }
                _frameIndex = newFrameIndex;
            }

            if (useUnscaledTime)
            {
                _time += Time.unscaledDeltaTime;
            }
            else
            {
                _time += Time.deltaTime;
            }

            if (_finished)
            {
                OnFinish?.Invoke();
            }
        }

        private void LateUpdate()
        {
            if (_sprites == null)
            {
                _renderer.sprite = null;
                return;
            }

            if (_finished)
            {
                if (hideOnFinish)
                {
                    _renderer.sprite = null;
                }
                return;
            }
            
            var spriteIndex = _frameIndex;
            if (reversed)
            {
                spriteIndex = _sprites.Length - spriteIndex - 1;
            }
            
            _renderer.sprite = _sprites[spriteIndex];
        }
    }
}
