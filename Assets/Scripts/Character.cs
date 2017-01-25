using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	public int directionCount = 8;
	public float speed = 3.5f;
	public float attackSpeed = 1.0f;
    public float useRange = 1f;
    public float attackRange = 1f;
    public float diameter = 1f;
    public bool run = false;

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
	public int direction = 0;

    Iso iso;
	IsoAnimator animator;
    SpriteRenderer spriteRenderer;
    List<Pathing.Step> path = new List<Pathing.Step>();
	float traveled = 0;
	int desiredDirection = 0;
    bool moving = false;
	bool attack = false;
    bool takingDamage = false;
    bool dying = false;
    bool dead = false;
    GameObject m_Target;
    Usable usable;
    Character targetCharacter;
    public int attackDamage = 30;
    public int health = 100;
    public int maxHealth = 100;
    Vector2 targetPoint;

    void Awake()
    {
		iso = GetComponent<Iso>();
		animator = GetComponent<IsoAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

	public void Use(Usable usable) {
        if (attack || takingDamage || dying || dead)
            return;
        targetPoint = usable.GetComponent<Iso>().pos;
		this.usable = usable;
        targetCharacter = null;
        moving = true;
    }

    public void GoTo(Vector2 target)
    {
        if (attack || takingDamage || dying || dead)
            return;

        moving = true;
        targetPoint = target;
        usable = null;
        targetCharacter = null;
    }

    public void Teleport(Vector2 target) {
		if (attack && takingDamage)
			return;
		if (Tilemap.instance[target]) {
			iso.pos = target;
		} else {
			var pathToTarget = Pathing.BuildPath(iso.pos, target, directionCount);
			if (pathToTarget.Count == 0)
				return;
			iso.pos = pathToTarget[pathToTarget.Count - 1].pos;
		}
		path.Clear();
		traveled = 0;
        moving = false;
	}

    public void Attack()
    {
        if (!dead && !dying && !attack && !takingDamage && direction == desiredDirection && !moving)
        {
            attack = true;
        }
    }

    public void Attack(Character targetCharacter)
    {
        if (attack || takingDamage || dead || dying)
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
        if (!takingDamage && !dead && !dying) {
            if (usable)
            {
                if (Vector2.Distance(usable.GetComponent<Iso>().pos, iso.pos) <= useRange + diameter / 2)
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
            }
        }

        MoveToTargetPoint();
        Turn();
	}

    void LateUpdate()
    {
        UpdateAnimation();
    }

    void Turn()
    {
        if (!dead && !dying && !attack && !takingDamage && direction != desiredDirection)
        {
            int diff = (int)Mathf.Sign(Tools.ShortestDelta(direction, desiredDirection, directionCount));
            direction = (direction + diff + directionCount) % directionCount;
        }
    }

	void MoveAlongPath() {
		if (path.Count == 0 || !moving || attack || takingDamage || dead || dying)
			return;

		Vector2 step = path[0].direction;
		float stepLen = step.magnitude;

        float distance = speed * Time.deltaTime;
		while (traveled + distance >= stepLen) {
			float firstPart = stepLen - traveled;
            Vector2 newPos = iso.pos + step.normalized * firstPart;
            iso.pos = newPos;
			distance -= firstPart;
			traveled += firstPart - stepLen;
			path.RemoveAt(0);
            if (path.Count > 0)
            {
                step = path[0].direction;
            }
		}
		if (path.Count > 0) {
			traveled += distance;
			iso.pos += step.normalized * distance;
		}

        if (path.Count == 0) {
			traveled = 0;
		}
        else
        {
            desiredDirection = path[0].directionIndex;
        }
    }

    void MoveToTargetPoint()
    {
        if (!moving)
            return;

        bool directlyAccesible = !Tilemap.Raycast(iso.pos, targetPoint, maxRayLength: 2.0f);
        if (directlyAccesible)
        {
            var dir = (targetPoint - iso.pos).normalized;
            float distance = speed * Time.deltaTime;
            iso.pos += dir * distance;
            desiredDirection = Iso.Direction(iso.pos, targetPoint, directionCount);
        }
        else
        {
            var newPath = Pathing.BuildPath(iso.pos, targetPoint, directionCount);
            if (path.Count == 0 || newPath.Count == 0 || newPath[newPath.Count - 1].pos != path[path.Count - 1].pos)
            {
                AbortPath();
                path.AddRange(newPath);
            }
            if (path.Count == 0)
                moving = false;
            Pathing.DebugDrawPath(iso.pos, path);
            MoveAlongPath();
        }

        if (usable == null && targetCharacter == null && Vector2.Distance(iso.pos, targetPoint) < 1)
        {
            moving = false;
        }
    }

    void UpdateAnimation() {
        string animation;
		animator.speed = 1.0f;
        if (dying || dead)
        {
            animation = "Death";
        }
        else if (attack)
        {
            animation = "Attack";
			animator.speed = attackSpeed;
        }
        else if (takingDamage)
        {
            animation = "TakeDamage";
        }
        else if (moving)
        {
            animation = run ? "Run" : "Walk";
        }
        else
        {
            animation = "Idle";
        }

        animator.SetState(animation);
    }

	public void LookAt(Vector3 target)
	{
        if (!moving)
            desiredDirection = Iso.Direction(iso.pos, target, directionCount);
    }

    public void LookAtImmidietly(Vector3 target)
    {
        direction = desiredDirection = Iso.Direction(iso.pos, target, directionCount);
    }

    public void TakeDamage(Character originator, int damage)
    {
        health -= damage;
        if (health > 0)
        {
            if (OnTakeDamage != null)
                OnTakeDamage(originator, damage);
            takingDamage = true;
            attack = false;
        }
        else
        {
            LookAtImmidietly(originator.iso.pos);
            dying = true;
            attack = false;
        }
        moving = false;
        targetCharacter = null;
    }

    void OnAnimationMiddle()
    {
        if (attack)
        {
            if (targetCharacter)
            {
                targetCharacter.TakeDamage(this, attackDamage);
                targetCharacter = null;
                m_Target = null;
            }
        }
    }

    void OnAnimationFinish() {
        attack = false;
        takingDamage = false;
        if (dying)
        {
            spriteRenderer.sortingLayerName = "OnFloor";
            dying = false;
            dead = true;
        }
        UpdateAnimation();
    }
}
