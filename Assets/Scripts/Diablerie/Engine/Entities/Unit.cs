using System.Collections.Generic;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Iso))]
    public class Unit : Entity
    {
        public const int DirectionCount = 32;
    
        [System.NonSerialized]
        public MonStat monStat;
        public Equipment equip;
        public float walkSpeed = 3.5f;
        public float runSpeed = 6f;
        public float attackRange = 1.5f;
        public int size = 2;
        public bool run = false;
        public int level = 1;
        public Party party = Party.Good;

        public string basePath;
        public string token;
        public string weaponClass;

        private static float turnSpeed = 3.5f; // full rotations per second

        private int _directionIndex = 0;
        private float _direction = 0;
    
        public Iso iso; // readonly
        private List<Pathing.Step> path = new List<Pathing.Step>();
        private float _traveled = 0;
        private int _desiredDirection = 0;
        private bool _moving = false;
        private bool _dying = false;
        private bool _dead = false;
        private bool _usingSkill = false;
        private bool _resurrecting = false;
        public string overrideMode;
        private string _mode;
        public bool killable = true;
        public int health = 100;
        public int maxHealth = 100;
        public int mana = 100;
        public int maxMana = 100;
        private bool _hasMoved = false;
        private SkillInfo _skillInfo;
        private Item _skillWeapon;
        private Entity _targetEntity;
        private Entity _operatingWithEntity;
        private Unit _targetUnit;
        private Vector2 _targetPoint;
        
        // Animation
        private float _animationTime;
        private int _animationFrame;
        private AnimData _animData;
        
        public string Mode => _mode;

        public int DirectionIndex => _directionIndex;

        public int AnimationFrame => _animationFrame;

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            iso = GetComponent<Iso>();
        }

        protected override void Start()
        {
            base.Start();
            CollisionMap.Move(iso.pos, iso.pos, size, gameObject);
            Events.InvokeUnitInitialized(this);
            UpdateMode();
        }
        
        void Update()
        {
            TryUseSkill();
            OperateWithTarget();
            MoveToTargetPoint();
            Turn();
            UpdateAnimation();
            Iso.DebugDrawTile(iso.pos, party == Party.Good ? Color.green : Color.red, 0.3f);
        }

        #endregion
        
        #region Commands and mode transitions

        public void LookAt(Vector3 target)
        {
            if (!_moving)
                _desiredDirection = Iso.Direction(iso.pos, target, DirectionCount);
        }

        public void LookAtImmediately(Vector3 target)
        {
            _directionIndex = _desiredDirection = Iso.Direction(iso.pos, target, DirectionCount);
        }

        public void Use(Entity entity)
        {
            if (_dying || _dead || _resurrecting || _usingSkill || overrideMode != null)
                return;
            _targetPoint = Iso.MapToIso(entity.transform.position);
            _targetEntity = entity;
            _targetUnit = null;
            _skillInfo = null;
        }

        public void GoTo(Vector2 target)
        {
            if (_dying || _dead || _resurrecting || _usingSkill || overrideMode != null)
                return;

            if (monStat != null && !monStat.ext.hasMode[2])
                return;

            _moving = true;
            _targetPoint = target;
            _targetEntity = null;
            _targetUnit = null;
            _skillInfo = null;
        }

        public void InstantMove(Vector2 target)
        {
            Vector3 newPos;
            if (CollisionMap.Fit(target, out newPos, size))
            {
                CollisionMap.Move(iso.pos, newPos, size, gameObject);
                iso.pos = newPos;
                _moving = false;
            }
            else
            {
                Debug.LogWarning("Can't move unit - doesn't fit");
            }
        }

        public void StopMoving()
        {
            _moving = false;
        }

        public void UseSkill(SkillInfo skillInfo, Vector3 target)
        {
            if (_dead || _dying || _resurrecting || _usingSkill || overrideMode != null)
                return;

            _skillWeapon = equip.GetWeapon();
            _targetPoint = target;
            _skillInfo = skillInfo;
        }

        public void UseSkill(SkillInfo skillInfo, Unit target)
        {
            if (_dead || _dying || _resurrecting || _usingSkill || overrideMode != null)
                return;

            if (!CanUseSkill(skillInfo, target))
                return;

            _targetEntity = null;
            _targetUnit = target;
            _skillInfo = skillInfo;
        }

        public void Kill(Unit killer = null)
        {
            _dying = true;
            _targetUnit = null;
            _moving = false;
            _usingSkill = false;
            _skillInfo = null;
            
            CollisionMap.SetPassable(Iso.Snap(iso.pos), size, size, true, gameObject);

            Events.InvokeUnitDied(this, killer);
        }

        public void GetHit()
        {
            overrideMode = "GH";
            _targetUnit = null;
            _moving = false;
            _usingSkill = false;
            _skillInfo = null;
            UpdateMode();
        }
        
        #endregion
    
        #region Commands execution
        
        public bool CanUseSkill(SkillInfo skillInfo, Unit target)
        {
            if (!skillInfo.targetAlly && target.party == party)
                return false;
            if (!skillInfo.targetCorpse && (target._dead || target._dying))
                return false;
            return true;
        }

        private void TryUseSkill()
        {
            if (_dead || _dying || _resurrecting || _usingSkill || _skillInfo == null || overrideMode != null)
                return;

            if (_targetUnit != null)
            {
                _targetPoint = _targetUnit.iso.pos;
            }

            bool ranged = _skillWeapon != null && _skillWeapon.info.type.shoots != null;

            if (ranged || _skillInfo.IsRangeOk(this, _targetUnit, _targetPoint))
            {
                LookAtImmediately(_targetPoint);
                _usingSkill = true;
                _moving = false;
                Events.InvokeUnitStartedSkill(this, _skillInfo);
                
                if (_skillInfo.castOverlay != null)
                {
                    // TODO set overlay speed to spell cast rate
                    Overlay.Create(gameObject, loop: false, overlayInfo: _skillInfo.castOverlay);
                }
            }
            else
            {
                _moving = true;
            }
        }

        private void OperateWithTarget()
        {
            if (_dead || _dying || _resurrecting || _usingSkill || overrideMode != null)
                return;
        
            if (_targetEntity)
            {
                var distance = Vector2.Distance(iso.pos, Iso.MapToIso(_targetEntity.transform.position));
                if (distance <= size + _targetEntity.operateRange)
                {
                    _moving = false;
                    if (_targetEntity.isAttackable)
                    {
                        _operatingWithEntity = _targetEntity;
                        overrideMode = "KK";
                    }
                    else
                    {
                        _targetEntity.Operate(this);
                    }
                    _targetEntity = null;
                    PlayerController.instance.FlushInput();  // Very strange to see it here, remove
                }
                else
                {
                    _moving = true;
                }
            }
        }

        private void MoveToTargetPoint()
        {
            _hasMoved = false;
            if (!_moving || _dead || _dying || _resurrecting || _usingSkill || overrideMode != null)
                return;

            var newPath = Pathing.BuildPath(iso.pos, _targetPoint, size: size, self: gameObject);
            if (newPath.Count == 0)
            {
                _moving = false;
                return;
            }
            _hasMoved = true;
            if (path.Count == 0 || newPath[newPath.Count - 1].pos != path[path.Count - 1].pos)
            {
                AbortPath();
                path.AddRange(newPath);
            }
        
            if (path.Count == 1 && Vector2.Distance(path[0].pos, _targetPoint) < 1.0f)
            {
                // smooth straightforward movement
                var pathStep = path[0];
                pathStep.pos = _targetPoint;
                pathStep.direction = _targetPoint - iso.pos;
                path[0] = pathStep;
            }

            MoveAlongPath();
        }

        private void Turn()
        {
            if (!_dead && !_dying && !_resurrecting && !_usingSkill && overrideMode == null && _directionIndex != _desiredDirection)
            {
                float diff = Tools.ShortestDelta(_directionIndex, _desiredDirection, DirectionCount);
                float delta = Mathf.Abs(diff);
                _direction += Mathf.Clamp(Mathf.Sign(diff) * turnSpeed * Time.deltaTime * DirectionCount, -delta, delta);
                _direction = Tools.Mod(_direction + DirectionCount, DirectionCount);
                _directionIndex = Mathf.RoundToInt(_direction) % DirectionCount;
            }
        }

        private void MoveAlongPath()
        {
            Vector2 step = path[0].direction;
            float stepLen = step.magnitude;
            Vector2 movement = new Vector3();
            float speed = run ? runSpeed : walkSpeed;
            float distance = speed * Time.deltaTime;

            while (_traveled + distance >= stepLen)
            {
                float part = stepLen - _traveled;
                movement += step.normalized * part;
                distance -= part;
                _traveled = 0;
                path.RemoveAt(0);
                if (path.Count > 0)
                {
                    step = path[0].direction;
                    stepLen = step.magnitude;
                }
                else
                {
                    break;
                }
            }
            if (path.Count > 0)
            {
                _traveled += distance;
                movement += step.normalized * distance;
            }
        
            Move(movement);

            if (path.Count == 0)
            {
                _traveled = 0;
            }
            else if (_moving)
            {
                _desiredDirection = Iso.Direction(iso.pos, iso.pos + step, DirectionCount);
            }
        }

        private void AbortPath()
        {
            path.Clear();
            _traveled = 0;
        }

        private bool Move(Vector2 movement)
        {
            var newPos = iso.pos + movement;
            CollisionMap.Move(iso.pos, newPos, size, gameObject);
            iso.pos = newPos;
            return true;
        }
        
        #endregion

        #region Animation
        
        private void UpdateAnimation()
        {
            UpdateMode();
            _animationTime += Time.deltaTime;
            while (_animationTime >= _animData.frameDuration && _animData.frameDuration > 0)
            {
                _animationTime -= _animData.frameDuration;
                if (_animationFrame < _animData.framesPerDir)
                    _animationFrame += 1;
                if (_animationFrame == _animData.framesPerDir / 2)
                    OnAnimationMiddle();
                if (_animationFrame == _animData.framesPerDir)
                {
                    OnAnimationFinish();
                    _animationFrame = 0;
                }
            }
        }
        
        void OnAnimationMiddle()
        {
            if (_usingSkill)
            {
                _skillInfo.Do(this, _targetUnit, _targetPoint);
            }
            else if (_operatingWithEntity != null && overrideMode == "KK")
            {
                _operatingWithEntity.Operate(this);
            }
        }

        void OnAnimationFinish()
        {
            _operatingWithEntity = null;
            _targetUnit = null;
            _usingSkill = false;
            _resurrecting = false;
            _skillInfo = null;
            overrideMode = null;
            if (_dying)
            {
                _dying = false;
                _dead = true;
            }
            UpdateMode();
        }

        private void UpdateMode()
        {
            string newMode = CalcMode();
            if (_mode != newMode)
            {
                _mode = newMode;
                _animationFrame = 0;
                AnimData.Find(token + _mode + weaponClass, ref _animData);
                _animData.frameDuration = _animData.GetCorrectedFrameDuration(token, _mode);
            }
        }

        private string CalcMode()
        {
            if (_resurrecting && monStat != null)
            {
                return monStat.ext.resurrectMode;
            }
            if (_dying)
            {
                return "DT";
            }
            if (_dead)
            {
                return "DD";
            }
            if (_hasMoved)
            {
                return run ? "RN" : "WL";
            }
            if (_usingSkill)
            {
                return _skillInfo.anim;
            }
            if (overrideMode != null)
            {
                return overrideMode;
            }
            return "NU";
        }

        #endregion

        public void Hit(int damage, Unit originator = null)
        {
            if (_dying || _dead || _resurrecting)
                return;

            if (!killable)
                return;

            if (originator != null && originator.party == party)
                return;

            health -= damage;
            if (health > 0)
            {
                if (damage > maxHealth * 0.1f)
                    GetHit();
                Events.InvokeUnitTookDamage(this, damage);
            }
            else
            {
                if (originator)
                    LookAtImmediately(originator.iso.pos);
                Kill(originator);
            }
        }

        public override Vector2 titleOffset
        {
            get
            {
                if (monStat != null)
                    return new Vector2(0, monStat.ext.pixHeight);
                else
                    return new Vector2(0, (int)(bounds.size.y * Iso.pixelsPerUnit) + 10);
            }
        }

        public override bool selectable
        {
            get
            {
                bool selectable = !_dead && !_dying;
                if (selectable && monStat != null)
                    selectable = monStat.interact || (!monStat.npc && monStat.killable);
                return selectable;
            }
        }

        public override void Operate(Unit unit)
        {
            Events.InvokeUnitInteractionStarted(this, unit);
        }
    }
}
