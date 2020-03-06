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

        readonly static string[] gear = { "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT", "LIT" };

        private int _mode;
        private COFAnimator _animator;
        private Iso _iso;

        public ObjectInfo info => objectInfo;

        public override bool isAttackable => objectInfo.isAttackable;

        public override Vector2 titleOffset => new Vector2(0, -objectInfo.nameOffset);

        public override float operateRange => objectInfo.operateRange;

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

        void SetMode(string modeName)
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

        static string[] treasureClassLetters = new string[] { "A", "B", "C" };

        public override void Operate(Unit unit)
        {
            Debug.Log(unit.name + " use " + name + " (operateFn " + objectInfo.operateFn + ")");

            if (objectInfo.operateFn == 1 // bed, caskets
                || objectInfo.operateFn == 3 // urns
                || objectInfo.operateFn == 4 // chests
                || objectInfo.operateFn == 5 // barrels
                || objectInfo.operateFn == 14 // crates
                || objectInfo.operateFn == 51 // jungle objects
            )
            {
                AudioManager.instance.Play("object_chest_large");

                var levelInfo = LevelInfo.sheet[85]; // todo determine current level
                string tc = "Act " + (levelInfo.act + 1);
                var actLevels = LevelInfo.byAct[levelInfo.act];
                int lowest = actLevels[0].id;
                int highest = actLevels[actLevels.Count - 1].id;
                int letterIndex = (levelInfo.id - lowest) / ((highest - lowest + 1) / 3);
                string letter = treasureClassLetters[letterIndex];
                tc += " Chest " + letter;
                Debug.Log(tc);
                ItemDrop.Drop(tc, transform.position, levelInfo.id);
                SetMode("OP");
            }
            else if (objectInfo.operateFn == 23)
            {
                // waypoint
                if (COF.ModeNames[2][_mode] != "OP")
                {
                    AudioManager.instance.Play("object_waypoint_open");
                    SetMode("OP");
                }
            }
            else
            {
                SetMode("OP");
            }
        }

        public override bool selectable => objectInfo.draw && objectInfo.selectable[_mode];
    }
}
