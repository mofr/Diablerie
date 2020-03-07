using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [RequireComponent(typeof(Iso))]
    [ExecuteInEditMode]
    [System.Diagnostics.DebuggerDisplay("{name}")]
    public class StaticObject : Entity
    {
        public ObjectInfo objectInfo;

        private int _mode;
        private Iso _iso;
        private float _animationTime;
        private float _animationDuration;

        public ObjectInfo info => objectInfo;
        public override bool isAttackable => objectInfo.isAttackable;
        public override Vector2 titleOffset => new Vector2(0, -objectInfo.nameOffset);
        public override float operateRange => objectInfo.operateRange;
        public int ModeIndex => _mode;
        public float AnimationTime => _animationTime;
        public float AnimationDuration => _animationDuration;

        protected override void Awake()
        {
            base.Awake();
            _iso = GetComponent<Iso>();
        }

        private void Update()
        {
            _animationTime += Time.deltaTime;
            if (_animationTime >= _animationDuration)
            {
                _animationTime = 0;
                FinishAnimation();
            }
        }

        void FinishAnimation()
        {
            if (_mode == 1)
            {
                SetMode("ON");
            }
        }

        public void SetMode(string modeName)
        {
            int newMode = System.Array.IndexOf(COF.StaticObjectModes, modeName);
            if (newMode == -1 || !objectInfo.mode[newMode])
            {
                Debug.LogWarning("Failed to set mode '" + modeName + "' of object " + name);
                return;
            }

            if (objectInfo.hasCollision[_mode])
                CollisionMap.SetPassable(Iso.Snap(_iso.pos), objectInfo.sizeX, objectInfo.sizeY, true, gameObject);
            if (objectInfo.hasCollision[newMode])
                CollisionMap.SetPassable(Iso.Snap(_iso.pos), objectInfo.sizeX, objectInfo.sizeY, false, gameObject);

            _mode = newMode;
            _animationTime = 0;
            _animationDuration = objectInfo.frameCount[_mode] * objectInfo.frameDuration[_mode];
        }

        public override void Operate(Unit unit)
        {
            Events.InvokeStaticObjectOperate(this, unit);
        }

        public override bool selectable => objectInfo.draw && objectInfo.selectable[_mode];
    }
}
