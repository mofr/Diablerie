using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        public bool loop = true;
        public float fps = 25;
        public event System.Action OnFinish;

        new SpriteRenderer renderer;
        Sprite[] _sprites;
        float time = 0;
        int frameIndex = 0;
        bool _finished = false;

        int triggerFrame = -1;
        System.Action triggerAction;

        public Sprite[] sprites
        {
            get
            {
                return _sprites;
            }

            set
            {
                _sprites = value;
            }
        }

        public void Restart()
        {
            time = 0;
            frameIndex = 0;
            _finished = false;
        }

        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            renderer.material = Materials.normal;
        }

        public void SetTrigger(int frame, System.Action action)
        {
            triggerFrame = frame;
            triggerAction = action;
        }

        void Update()
        {
            if (_sprites == null || _finished)
                return;

            int newFrameIndex = (int)(time * fps);
            if (newFrameIndex >= sprites.Length)
            {
                if (loop)
                {
                    newFrameIndex %= sprites.Length;
                }
                else
                {
                    newFrameIndex = sprites.Length - 1;
                    if (!_finished)
                    {
                        _finished = true;
                        if (OnFinish != null)
                            OnFinish();
                    }
                }
            }

            if (frameIndex != newFrameIndex)
            {
                if (frameIndex < triggerFrame && newFrameIndex >= triggerFrame)
                    triggerAction();
                frameIndex = newFrameIndex;
            }
            
            time += Time.deltaTime;
        }

        void LateUpdate()
        {
            if (_sprites == null || _finished)
                return;
            renderer.sprite = _sprites[frameIndex];
        }
    }
}
