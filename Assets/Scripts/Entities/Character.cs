using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Iso))]
[RequireComponent(typeof(COFAnimator))]
public class Character : Entity
{
    [System.NonSerialized]
    public MonStat monStat;
    [System.NonSerialized]
    public CharStat charStat;
    public Equipment equip;
    public int directionCount = 8;
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

    static float turnSpeed = 3.5f; // full rotations per second

    public delegate void TakeDamageHandler(Character originator, int damage);
    public event TakeDamageHandler OnTakeDamage;

    public delegate void DeathHandler(Character target, Character killer);
    public static event DeathHandler OnDeath;

    [HideInInspector]
    public int directionIndex = 0;
    float direction = 0;
    
    public Iso iso; // readonly
    COFAnimator animator;
    List<Pathing.Step> path = new List<Pathing.Step>();
    float traveled = 0;
    int desiredDirection = 0;
    bool moving = false;
    bool dying = false;
    bool dead = false;
    bool usingSkill = false;
    public bool ressurecting = false;
    public string overrideMode;
    public int attackDamage = 30;
    public int health = 100;
    public int maxHealth = 100;
    public int mana = 100;
    public int maxMana = 100;
    bool hasMoved = false;
    SkillInfo skillInfo;
    Item skillWeapon;

    Entity targetEntity;
    Character targetCharacter;
    Vector2 targetPoint;

    void Awake()
    {
        iso = GetComponent<Iso>();
        animator = GetComponent<COFAnimator>();
    }

    protected override void Start()
    {
        base.Start();
        CollisionMap.Move(iso.pos, iso.pos, size, gameObject);
        if (monStat != null)
            AudioManager.instance.Play(monStat.sound.init, transform);
    }

    public void Use(Entity entity)
    {
        if (dying || dead || ressurecting || usingSkill || overrideMode != null)
            return;
        targetPoint = Iso.MapToIso(entity.transform.position);
        targetEntity = entity;
        targetCharacter = null;
        skillInfo = null;
    }

    public void GoTo(Vector2 target)
    {
        if (dying || dead || ressurecting || usingSkill || overrideMode != null)
            return;

        if (monStat != null && !monStat.ext.hasMode[2])
            return;

        moving = true;
        targetPoint = target;
        targetEntity = null;
        targetCharacter = null;
        skillInfo = null;
    }

    public void InstantMove(Vector2 target)
    {
        Vector3 newPos;
        if (CollisionMap.Fit(target, out newPos, size))
        {
            CollisionMap.Move(iso.pos, newPos, size, gameObject);
            iso.pos = newPos;
            moving = false;
        }
        else
        {
            Debug.LogWarning("Can't move character - doesn't fit");
        }
    }

    public void UseSkill(SkillInfo skillInfo, Vector3 target)
    {
        if (dead || dying || ressurecting || usingSkill || overrideMode != null)
            return;

        skillWeapon = equip.GetWeapon();
        targetPoint = target;
        this.skillInfo = skillInfo;
    }

    public void UseSkill(SkillInfo skillInfo, Character target)
    {
        if (dead || dying || ressurecting || usingSkill || overrideMode != null)
            return;

        targetEntity = null;
        targetCharacter = target;
        this.skillInfo = skillInfo;
    }

    void AbortPath()
    {
        path.Clear();
        traveled = 0;
    }

    void OperateWithTarget()
    {
        if (dead || dying || ressurecting || usingSkill || overrideMode != null)
            return;
        
        if (targetEntity)
        {
            var distance = Vector2.Distance(iso.pos, Iso.MapToIso(targetEntity.transform.position));
            if (distance <= size + targetEntity.operateRange)
            {
                var localEntity = targetEntity;
                moving = false;
                targetEntity = null;
                localEntity.Operate(this);
                PlayerController.instance.FlushInput();
            }
            else
            {
                moving = true;
            }
        }
    }

    void TryUseSkill()
    {
        if (dead || dying || ressurecting || usingSkill || skillInfo == null || overrideMode != null)
            return;

        if (targetCharacter != null)
        {
            targetPoint = targetCharacter.iso.pos;
        }

        bool ranged = skillWeapon != null && skillWeapon.info.type.shoots != null;

        if (ranged || skillInfo.IsRangeOk(this, targetCharacter, targetPoint))
        {
            LookAtImmediately(targetPoint);
            usingSkill = true;
            moving = false;
            if (skillInfo.castOverlay != null)
            {
                // TODO set overlay speed to spell cast rate
                Overlay.Create(gameObject, loop: false, overlayInfo: skillInfo.castOverlay);
            }

            AudioManager.instance.Play(skillInfo.startSound, transform);

            if (skillInfo == SkillInfo.Attack)
            {
                if (monStat != null)
                {
                    AudioManager.instance.Play(monStat.sound.weapon1, transform, 
                        delay: monStat.sound.weapon1Delay, volume: monStat.sound.weapon1Volume);
                    AudioManager.instance.Play(monStat.sound.attack1, transform, 
                        delay: monStat.sound.attack1Delay);
                }
                else
                {
                    Item weapon = equip == null ? null : equip.GetWeapon();
                    WeaponHitClass hitClass = WeaponHitClass.HandToHand;
                    if (weapon != null)
                        hitClass = weapon.info.weapon.hitClass;
                    AudioManager.instance.Play(hitClass.sound, transform);
                }
            }
        }
        else
        {
            moving = true;
        }
    }

    void Update()
    {
        TryUseSkill();
        OperateWithTarget();
        hasMoved = false;
        MoveToTargetPoint();
        Turn();
        Iso.DebugDrawTile(iso.pos, party == Party.Good ? Color.green : Color.red, 0.3f);
    }

    private void LateUpdate()
    {
        UpdateAnimation();
    }

    void Turn()
    {
        if (!dead && !dying && !ressurecting && !usingSkill && overrideMode == null && directionIndex != desiredDirection)
        {
            float diff = Tools.ShortestDelta(directionIndex, desiredDirection, directionCount);
            float delta = Mathf.Abs(diff);
            direction += Mathf.Clamp(Mathf.Sign(diff) * turnSpeed * Time.deltaTime * directionCount, -delta, delta);
            direction = Tools.Mod(direction + directionCount, directionCount);
            directionIndex = Mathf.RoundToInt(direction) % directionCount;
        }
    }

    void MoveAlongPath()
    {
        Vector2 step = path[0].direction;
        float stepLen = step.magnitude;
        Vector2 movement = new Vector3();
        float speed = run ? runSpeed : walkSpeed;
        float distance = speed * Time.deltaTime;

        while (traveled + distance >= stepLen)
        {
            float part = stepLen - traveled;
            movement += step.normalized * part;
            distance -= part;
            traveled = 0;
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
            traveled += distance;
            movement += step.normalized * distance;
        }
        
        Move(movement);

        if (path.Count == 0)
        {
            traveled = 0;
        }
        else if (moving)
        {
            desiredDirection = Iso.Direction(iso.pos, iso.pos + step, directionCount);
        }
    }

    void MoveToTargetPoint()
    {
        if (!moving || dead || dying || ressurecting || usingSkill || overrideMode != null)
            return;

        var newPath = Pathing.BuildPath(iso.pos, targetPoint, size: size, self: gameObject);
        if (newPath.Count == 0)
        {
            moving = false;
            return;
        }
        if (path.Count == 0 || newPath[newPath.Count - 1].pos != path[path.Count - 1].pos)
        {
            AbortPath();
            path.AddRange(newPath);
        }
        
        if (path.Count == 1 && Vector2.Distance(path[0].pos, targetPoint) < 1.0f)
        {
            // smooth straightforward movement
            var pathStep = path[0];
            pathStep.pos = targetPoint;
            pathStep.direction = targetPoint - iso.pos;
            path[0] = pathStep;
        }

        MoveAlongPath();
    }

    bool Move(Vector2 movement)
    {
        var newPos = iso.pos + movement;
        CollisionMap.Move(iso.pos, newPos, size, gameObject);
        iso.pos = newPos;
        hasMoved = true;
        return true;
    }

    public string Mode
    {
        get
        {
            if (ressurecting && monStat != null)
            {
                return monStat.ext.resurrectMode;
            }
            if (dying)
            {
                return "DT";
            }
            if (dead)
            {
                return "DD";
            }
            if (hasMoved)
            {
                return run ? "RN" : "WL";
            }
            if (usingSkill)
            {
                return skillInfo.anim;
            }
            if (overrideMode != null)
            {
                return overrideMode;
            }
            return "NU";
        }
    }

    void UpdateAnimation()
    {
        string mode = Mode;
        string weaponClass = this.weaponClass;
        animator.speed = 1.0f;
        animator.loop = true;
        if (mode == "DT" || mode == "DD")
        {
            weaponClass = "HTH";
            animator.loop = false;
        }

        animator.cof = COF.Load(basePath, token, weaponClass, mode);
        animator.direction = directionIndex;
    }

    public void LookAt(Vector3 target)
    {
        if (!moving)
            desiredDirection = Iso.Direction(iso.pos, target, directionCount);
    }

    public void LookAtImmediately(Vector3 target)
    {
        directionIndex = desiredDirection = Iso.Direction(iso.pos, target, directionCount);
    }

    public void TakeDamage(int damage, Character originator = null)
    {
        if (dying || dead || ressurecting)
            return;

        health -= damage;
        if (health > 0)
        {
            if (OnTakeDamage != null)
                OnTakeDamage(originator, damage);
            if (damage > maxHealth * 0.1f)
            {
                overrideMode = "GH";
                targetCharacter = null;
                moving = false;
                usingSkill = false;
                skillInfo = null;
            }

            if (monStat != null)
                AudioManager.instance.Play(monStat.sound.hit, transform, monStat.sound.hitDelay);
        }
        else
        {
            if (originator)
                LookAtImmediately(originator.iso.pos);
            dying = true;
            targetCharacter = null;
            moving = false;
            usingSkill = false;
            skillInfo = null;
            
            CollisionMap.SetPassable(Iso.Snap(iso.pos), size, size, true, gameObject);

            if (OnDeath != null)
                OnDeath(this, originator);

            if (monStat != null)
                AudioManager.instance.Play(monStat.sound.death, transform, monStat.sound.deathDelay);
        }
    }

    void OnAnimationMiddle()
    {
        if (usingSkill && animator.cof.mode == skillInfo.anim)
        {
            skillInfo.Do(this, targetCharacter, targetPoint);
        }
    }

    void OnAnimationFinish()
    {
        targetCharacter = null;
        usingSkill = false;
        ressurecting = false;
        skillInfo = null;
        overrideMode = null;
        if (dying)
        {
            dying = false;
            dead = true;
        }
        
        // It's needed to call here, otherwise animator can loop the finished animation few frames more than needed
        UpdateAnimation();
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

    void OnRenderObject()
    {
        bool selectable = !dead && !dying;
        if (selectable && monStat != null)
            selectable = monStat.interact || (!monStat.npc && monStat.killable);
        if (selectable)
            MouseSelection.Submit(this);
    }
}
