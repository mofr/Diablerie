using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Iso))]
[RequireComponent(typeof(COFAnimator))]
public class Character : Entity
{
    [System.NonSerialized]
    public MonStat monStat;
    public Equipment equip;
    public int directionCount = 8;
    public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float attackRange = 1.5f;
    public int size = 2;
    public bool run = false;

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
    bool takingDamage = false;
    bool dying = false;
    bool dead = false;
    bool usingSkill = false;
    public bool ressurecting = false;
    public int attackDamage = 30;
    public int health = 100;
    public int maxHealth = 100;
    bool hasMoved = false;
    SkillInfo skillInfo;

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
        if (takingDamage || dying || dead || ressurecting || usingSkill)
            return;
        targetPoint = Iso.MapToIso(entity.transform.position);
        targetEntity = entity;
        targetCharacter = null;
        skillInfo = null;
    }

    public void GoTo(Vector2 target)
    {
        if (takingDamage || dying || dead || ressurecting || usingSkill)
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
        if (takingDamage || dead || dying || ressurecting || usingSkill)
            return;

        if (skillInfo.targetableOnly)
        {
            GoTo(target);
            return;
        }

        targetPoint = target;
        this.skillInfo = skillInfo;
    }

    public void UseSkill(SkillInfo skillInfo, Character target)
    {
        if (takingDamage || dead || dying || ressurecting || usingSkill)
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
        if (takingDamage || dead || dying || ressurecting || usingSkill)
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
        if (takingDamage || dead || dying || ressurecting || usingSkill || skillInfo == null)
            return;

        if (targetCharacter != null)
        {
            targetPoint = targetCharacter.iso.pos;
        }

        if (skillInfo.IsRangeOk(this, targetCharacter, targetPoint))
        {
            LookAtImmidietly(targetPoint);
            usingSkill = true;
            moving = false;
            if (skillInfo.castOverlay != null)
                Overlay.Create(gameObject, skillInfo.castOverlay);
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
                    AudioManager.instance.Play(hitClass.sound, targetCharacter.transform.position);
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
        Iso.DebugDrawTile(iso.pos, 0.3f);
    }

    private void LateUpdate()
    {
        UpdateAnimation();
    }

    void Turn()
    {
        if (!dead && !dying && !takingDamage && directionIndex != desiredDirection && !ressurecting && !usingSkill)
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
        if (!moving || takingDamage || dead || dying || ressurecting || usingSkill)
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

    void UpdateAnimation()
    {
        string mode;
        string weaponClass = this.weaponClass;
        animator.speed = 1.0f;
        animator.loop = true;
        if (ressurecting && monStat != null)
        {
            mode = monStat.ext.resurrectMode;
        }
        else if (dying)
        {
            mode = "DT";
            weaponClass = "HTH";
            animator.loop = false;
        }
        else if (dead)
        {
            mode = "DD";
            weaponClass = "HTH";
            animator.loop = false;
        }
        else if (takingDamage)
        {
            mode = "GH";
        }
        else if (hasMoved)
        {
            mode = run ? "RN" : "WL";
        }
        else if (usingSkill)
        {
            mode = skillInfo.anim;
        }
        else
        {
            mode = "NU";
        }

        animator.cof = COF.Load(basePath, token, weaponClass, mode);
        animator.direction = directionIndex;
    }

    public void LookAt(Vector3 target)
    {
        if (!moving)
            desiredDirection = Iso.Direction(iso.pos, target, directionCount);
    }

    public void LookAtImmidietly(Vector3 target)
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
                takingDamage = true;
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
                LookAtImmidietly(originator.iso.pos);
            dying = true;
            targetCharacter = null;
            moving = false;
            usingSkill = false;
            skillInfo = null;

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
        takingDamage = false;
        ressurecting = false;
        skillInfo = null;
        if (dying)
        {
            dying = false;
            dead = true;
            CollisionMap.SetPassable(Iso.Snap(iso.pos), size, size, true, gameObject);
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

    void OnRenderObject()
    {   
        if (!dead && !dying)
            MouseSelection.Submit(this);
    }
}
