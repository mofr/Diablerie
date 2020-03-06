using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [RequireComponent(typeof(Iso))]
    [RequireComponent(typeof(COFAnimator))]
    [ExecuteInEditMode]
    [System.Diagnostics.DebuggerDisplay("{name}")]
    public class StaticObject : Entity
    {
        public string modeName = "NU";
        public ObjectInfo objectInfo;

        static readonly string[] gear = { "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT" };

        private int _mode;
        private COFAnimator _animator;
        private Iso _iso;

        public ObjectInfo info => objectInfo;

        public override bool isAttackable => objectInfo.isAttackable;

        public override Vector2 titleOffset => new Vector2(0, -objectInfo.nameOffset);

        public override float operateRange => objectInfo.operateRange;

        public int ModeIndex => _mode;

        protected override void Awake()
        {
            base.Awake();
            _iso = GetComponent<Iso>();
            _animator = GetComponent<COFAnimator>();
            _animator.equip = gear;
        }

        protected override void Start()
        {
            base.Start();
            SetMode(modeName);
        }

        void OnAnimationFinish()
        {
            if (_mode == 1)
            {
                SetMode("ON");
            }
        }

        public void SetMode(string modeName)
        {
            if (objectInfo.draw)
            {
                int newMode = System.Array.IndexOf(COF.ModeNames[2], modeName);
                if (newMode == -1 || !objectInfo.mode[newMode])
                {
                    Debug.LogWarning("Failed to set _mode '" + modeName + "' of object " + name);
                    return;
                }

                if (objectInfo.hasCollision[_mode])
                    CollisionMap.SetPassable(Iso.Snap(_iso.pos), objectInfo.sizeX, objectInfo.sizeY, true, gameObject);

                _mode = newMode;

                var cof = COF.Load(@"data\global\objects", objectInfo.token, "HTH", modeName);
                _animator.shadow = objectInfo.blocksLight[_mode];
                _animator.cof = cof;
                _animator.loop = objectInfo.cycleAnim[_mode];
                _animator.SetFrameRange(objectInfo.start[_mode], objectInfo.frameCount[_mode]);
                _animator.frameDuration = objectInfo.frameDuration[_mode];

                if (objectInfo.hasCollision[_mode])
                    CollisionMap.SetPassable(Iso.Snap(_iso.pos), objectInfo.sizeX, objectInfo.sizeY, false, gameObject);
            }
        }

        public override void Operate(Unit unit)
        {
            Events.InvokeStaticObjectOperate(this, unit);
        }

        public override bool selectable => objectInfo.draw && objectInfo.selectable[_mode];
    }
}
