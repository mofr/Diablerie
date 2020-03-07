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

        private StaticObjectMode _mode = StaticObjectMode.Neutral;
        private Iso _iso;
        private float _animationTime;
        private float _animationDuration;

        public ObjectInfo info => objectInfo;
        public override bool isAttackable => objectInfo.isAttackable;
        public override Vector2 titleOffset => new Vector2(0, -objectInfo.nameOffset);
        public override float operateRange => objectInfo.operateRange;
        public StaticObjectMode Mode => _mode;
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
            if (_mode == StaticObjectMode.Operating)
            {
                SetMode(StaticObjectMode.On);
            }
        }

        public void SetMode(StaticObjectMode mode)
        {
            if (!objectInfo.mode[mode.index])
            {
                Debug.LogWarning("Failed to set mode '" + mode + "' of object " + name);
                return;
            }

            if (objectInfo.hasCollision[_mode.index])
                CollisionMap.SetPassable(Iso.Snap(_iso.pos), objectInfo.sizeX, objectInfo.sizeY, true, gameObject);
            if (objectInfo.hasCollision[mode.index])
                CollisionMap.SetPassable(Iso.Snap(_iso.pos), objectInfo.sizeX, objectInfo.sizeY, false, gameObject);

            _mode = mode;
            _animationTime = 0;
            _animationDuration = objectInfo.frameCount[_mode.index] * objectInfo.frameDuration[_mode.index];
        }

        public override void Operate(Unit unit)
        {
            Events.InvokeStaticObjectOperate(this, unit);
        }

        public override bool selectable => objectInfo.draw && objectInfo.selectable[_mode.index];
    }
}
