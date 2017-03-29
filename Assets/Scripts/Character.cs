using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Iso))]
[RequireComponent(typeof(COFAnimator))]
public class Character : Entity
{
    [System.NonSerialized]
    public MonStat monStat;
    public int directionCount = 8;
	public float walkSpeed = 3.5f;
    public float runSpeed = 6f;
    public float attackSpeed = 1.0f;
    public float useRange = 1f;
    public float attackRange = 2.5f;
    public float diameter = 1f;
    public bool run = false;

    public string basePath;
    public string token;
    public string weaponClass;
    public string[] gear;

    static float turnSpeed = 3.5f; // full rotations per second

    public delegate void TakeDamageHandler(Character originator, int damage);
    public event TakeDamageHandler OnTakeDamage;

    [HideInInspector]
    public GameObject target
    {
        get
        {
            return m_Target;
        }

        set
        {
            var usable = value.GetComponent<Usable>();
            if (usable != null)
                Use(usable);
            else
            {
                var targetCharacter = value.GetComponent<Character>();
                if (targetCharacter) {
                    Attack(targetCharacter);
                }
                else
                {
                    return;
                }
            }

            m_Target = value;
        }
    }

    [HideInInspector]
	public int directionIndex = 0;
    float direction = 0;

    Iso iso;
	COFAnimator animator;
    List<Pathing.Step> path = new List<Pathing.Step>();
	float traveled = 0;
	int desiredDirection = 0;
    bool moving = false;
	bool attack = false;
    bool takingDamage = false;
    bool dying = false;
    bool dead = false;
    public bool ressurecting = false;
    GameObject m_Target;
    Usable usable;
    Character targetCharacter;
    public int attackDamage = 30;
    public int health = 100;
    public int maxHealth = 100;
    Vector2 targetPoint;
    bool hasMoved = false;

    void Awake()
    {
		iso = GetComponent<Iso>();
		animator = GetComponent<COFAnimator>();
    }

    protected override void Start()
    {
        base.Start();
        animator.gear = gear;
        CollisionMap.SetPassable(Iso.Snap(iso.pos), false);
    }

	public void Use(Usable usable) {
        if (attack || takingDamage || dying || dead || ressurecting)
            return;
        targetPoint = usable.GetComponent<Iso>().pos;
		this.usable = usable;
        targetCharacter = null;
        moving = true;
    }

    public void GoTo(Vector2 target)
    {
        if (attack || takingDamage || dying || dead || ressurecting)
            return;

        if (monStat != null && !monStat.ext.hasMode[2])
            return;

        moving = true;
        targetPoint = target;
        usable = null;
        targetCharacter = null;
    }

    public void Teleport(Vector2 target) {
		if (attack || takingDamage || ressurecting)
			return;

		if (CollisionMap.Passable(target)) {
			iso.pos = target;
		} else {
			var pathToTarget = Pathing.BuildPath(iso.pos, target);
			if (pathToTarget.Count == 0)
				return;
			iso.pos = pathToTarget[pathToTarget.Count - 1].pos;
		}
		path.Clear();
		traveled = 0;
        moving = false;
	}

    public void Attack(Vector3 target)
    {
        if (!dead && !dying && !attack && !takingDamage && directionIndex == desiredDirection && !moving && !ressurecting)
        {
            attack = true;
            targetPoint = target;
        }
    }

    public void Attack(Character targetCharacter)
    {
        if (attack || takingDamage || dead || dying || ressurecting)
            return;

        Iso targetIso = targetCharacter.GetComponent<Iso>();
        targetPoint = targetIso.pos;
        this.targetCharacter = targetCharacter;
        usable = null;
        moving = true;
    }

    void AbortPath()
    {
        m_Target = null;
        path.Clear();
        traveled = 0;
    }

	void Update() {
        if (!takingDamage && !dead && !dying && !ressurecting) {
            if (usable)
            {
                var hit = CollisionMap.Raycast(iso.pos, usable.GetComponent<Iso>().pos, maxRayLength: useRange + diameter / 2, ignore: gameObject);
                if (hit.gameObject == usable.gameObject)
                {
                    usable.Use();
                    moving = false;
                    usable = null;
                    m_Target = null;
                }
            }
            if (targetCharacter && !attack)
            {
                Vector2 target = targetCharacter.GetComponent<Iso>().pos;
                if (Vector2.Distance(target, iso.pos) <= attackRange + diameter / 2 + targetCharacter.diameter / 2)
                {
                    moving = false;
                    attack = true;
                    LookAtImmidietly(target);
                }
                else
                {
                    targetPoint = target;
                }
            }
        }

        hasMoved = false;
        MoveToTargetPoint();
        Turn();
	}

    void LateUpdate()
    {
        UpdateAnimation();
    }

    void Turn()
    {
        if (!dead && !dying && !attack && !takingDamage && directionIndex != desiredDirection && !ressurecting)
        {
            float diff = Tools.ShortestDelta(directionIndex, desiredDirection, directionCount);
            float delta = Mathf.Abs(diff);
            direction += Mathf.Clamp(Mathf.Sign(diff) * turnSpeed * Time.deltaTime * directionCount, -delta, delta);
            direction = Tools.Mod(direction + directionCount, directionCount);
            directionIndex = Mathf.RoundToInt(direction) % directionCount;
        }
    }

	void MoveAlongPath() {
		if (path.Count == 0 || !moving || attack || takingDamage || dead || dying || ressurecting)
			return;

		Vector2 step = path[0].direction;
		float stepLen = step.magnitude;
        Vector2 movement = new Vector3();
        float speed = run ? runSpeed : walkSpeed;
        float distance = speed * Time.deltaTime;

		while (traveled + distance >= stepLen) {
			float part = stepLen - traveled;
            movement += step.normalized * part;
            distance -= part;
            traveled = 0;
			path.RemoveAt(0);
            if (path.Count > 0)
            {
                step = path[0].direction;
            }
		}
		if (path.Count > 0) {
			traveled += distance;
            movement += step.normalized * distance;
        }

        Move(movement);

        if (path.Count == 0) {
			traveled = 0;
		}
        else if (moving)
        {
            desiredDirection = Iso.Direction(iso.pos, iso.pos + step, directionCount);
        }
    }

    void MoveToTargetPoint()
    {
        if (!moving)
            return;

        var newPath = Pathing.BuildPath(iso.pos, targetPoint, self: gameObject);
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
        Pathing.DebugDrawPath(iso.pos, path);
        if (path.Count == 1 && Vector2.Distance(path[0].pos, targetPoint) < 1.0f)
        {
            var dir = (targetPoint - iso.pos).normalized;
            float speed = run ? runSpeed : walkSpeed;
            var movement = dir * speed * Time.deltaTime;

            if (Move(movement))
                desiredDirection = Iso.Direction(iso.pos, targetPoint, directionCount);
        }
        else
        {
            MoveAlongPath();
        }
    }

    bool Move(Vector2 movement)
    {
        var newPos = iso.pos + movement;
        CollisionMap.Move(iso.pos, newPos, gameObject);
        iso.pos = newPos;
        hasMoved = true;
        return true;
    }

    void UpdateAnimation() {
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
        else if (attack)
        {
            mode = "A1";
            animator.speed = attackSpeed;
        }
        else if (takingDamage)
        {
            mode = "GH";
            animator.loop = false;
        }
        else if (hasMoved)
        {
            mode = run ? "RN" : "WL";
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
        if (dead || ressurecting)
            return;

        health -= damage;
        if (health > 0)
        {
            if (OnTakeDamage != null)
                OnTakeDamage(originator, damage);
            if (damage > maxHealth * 0.3f)
            {
                takingDamage = true;
                attack = false;
                moving = false;
                targetCharacter = null;
            }
        }
        else
        {
            if (originator)
                LookAtImmidietly(originator.iso.pos);
            dying = true;
            attack = false;
            moving = false;
            targetCharacter = null;
        }
    }

    void OnAnimationMiddle()
    {
        if (attack)
        {
            if (targetCharacter == null)
            {
                var hit = CollisionMap.Raycast(iso.pos, targetPoint, rayLength: diameter / 2 + attackRange, ignore: gameObject, debug: true);
                if (hit.gameObject != null)
                {
                    targetCharacter = hit.gameObject.GetComponent<Character>();
                }
            }

            if (targetCharacter)
            {
                Vector2 target = targetCharacter.GetComponent<Iso>().pos;
                if (Vector2.Distance(target, iso.pos) <= attackRange + diameter / 2 + targetCharacter.diameter / 2)
                {
                    targetCharacter.TakeDamage(attackDamage, this);
                }
                targetCharacter = null;
                m_Target = null;
            }
        }
    }

    void OnAnimationFinish()
    {
        attack = false;
        takingDamage = false;
        ressurecting = false;
        if (dying)
        {
            dying = false;
            dead = true;
            CollisionMap.SetPassable(Iso.Snap(iso.pos), true);
        }
        UpdateAnimation();
    }

    public override string name
    {
        get
        {
            if (monStat != null)
                return monStat.nameStr;
            else
                return "monStat null";
        }
    }

    public override int nameOffset
    {
        get
        {
            if (monStat != null)
                return monStat.ext.pixHeight;
            else
                return (int)(bounds.size.y * Iso.pixelsPerUnit) + 10;
        }
    }

    void OnRenderObject()
    {
        if (!dead && !dying)
            MouseSelection.Submit(this);
    }
}
