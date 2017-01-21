using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	public int directionCount = 8;
	public float speed = 3.5f;
	public float attackSpeed = 1.0f;
    public float useRange = 1.5f;
    public float attackRange = 2.5f;
    public bool run = false;

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
	List<Pathing.Step> path = new List<Pathing.Step>();
	float traveled = 0;
	int targetDirection = 0;
	bool attack = false;
    bool takingDamage = false;
    bool dying = false;
    bool dead = false;
    GameObject m_Target;
    Usable usable;
    Character targetCharacter;
    int attackDamage = 30;
    int health = 100;
    int maxHealth = 100;

    void Start() {
		iso = GetComponent<Iso>();
		animator = GetComponent<IsoAnimator>();
	}

    void PathTo(Vector2 target, float minRange = 0.1f)
    {
        AbortMovement();
        Vector2 startPos = path.Count > 0 ? path[0].pos : iso.tilePos;
        path.AddRange(Pathing.BuildPath(Iso.Snap(startPos), target, directionCount, minRange));
    }

	public void Use(Usable usable) {
        if (attack || takingDamage)
            return;
        PathTo(usable.GetComponent<Iso>().tilePos, useRange);
		this.usable = usable;
	}

	public void GoTo(Vector2 target) {
		if (attack || takingDamage)
			return;

        PathTo(target);
	}

	public void Teleport(Vector2 target) {
		if (attack && takingDamage)
			return;
		if (Tilemap.instance[target]) {
			iso.pos = target;
			iso.tilePos = target;
		} else {
			var pathToTarget = Pathing.BuildPath(Iso.Snap(iso.tilePos), target, directionCount);
			if (pathToTarget.Count == 0)
				return;
			iso.pos = pathToTarget[pathToTarget.Count - 1].pos;
			iso.tilePos = iso.pos;
		}
		path.Clear();
		traveled = 0;
	}

    void AbortMovement()
    {
        m_Target = null;
        usable = null;
        targetCharacter = null;

        if (path.Count > 0)
        {
            var firstStep = path[0];
            path.Clear();
            path.Add(firstStep);
        }
        else {
            path.Clear();
            traveled = 0;
        }
    }

	void Update() {
		Iso.DebugDrawTile(iso.tilePos);
		Pathing.DebugDrawPath(path);

		Move();

		if (path.Count == 0) {
            if (usable)
            {
                if (Vector2.Distance(usable.GetComponent<Iso>().tilePos, iso.tilePos) <= useRange)
                    usable.Use();
                usable = null;
                m_Target = null;
            }
            if (targetCharacter && !attack)
            {
                Vector2 target = targetCharacter.GetComponent<Iso>().tilePos;
                if (Vector2.Distance(target, iso.tilePos) <= attackRange)
                {
                    attack = true;
                    direction = Iso.Direction(iso.tilePos, target, directionCount);
                }
            }
        }

		UpdateAnimation();
	}

	void Move() {
		if (path.Count == 0)
			return;

		Vector2 step = path[0].direction;
		float stepLen = step.magnitude;

		float distance = speed * Time.deltaTime;
		while (traveled + distance >= stepLen) {
			float firstPart = stepLen - traveled;
			iso.pos += step.normalized * firstPart;
			distance -= firstPart;
			traveled += firstPart - stepLen;
			iso.tilePos += step;
			path.RemoveAt(0);
			if (path.Count > 0)
				step = path[0].direction;
		}
		if (path.Count > 0) {
			traveled += distance;
			iso.pos += step.normalized * distance;
		}

		if (path.Count == 0) {
			iso.pos.x = Mathf.Round(iso.pos.x);
			iso.pos.y = Mathf.Round(iso.pos.y);
			traveled = 0;
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
        else if (path.Count == 0)
        {
            animation = "Idle";
        }
        else
        {
            animation = run ? "Run" : "Walk";
            targetDirection = path[0].directionIndex;
        }

		if (!attack && !takingDamage && direction != targetDirection) {
			int diff = (int)Mathf.Sign(Tools.ShortestDelta(direction, targetDirection, directionCount));
			direction = (direction + diff + directionCount) % directionCount;
        }

        animator.SetState(animation);
    }

	public void LookAt(Vector3 target)
	{
        targetDirection = Iso.Direction(iso.tilePos, target, directionCount);
    }

    public void Attack() {
		if (!attack && !takingDamage && direction == targetDirection && path.Count == 0) {
			attack = true;
		}
	}

    public void Attack(Character targetCharacter)
    {
        if (attack || takingDamage)
            return;

        Iso targetIso = targetCharacter.GetComponent<Iso>();
        PathTo(targetIso.tilePos, attackRange);
        this.targetCharacter = targetCharacter;
    }

    public void TakeDamage(Character originator, int damage)
    {
        health -= damage;
        if (health > 0)
        {
            takingDamage = true;
        }
        else
        {
            direction = Iso.Direction(iso.tilePos, originator.iso.tilePos, directionCount);
            targetDirection = direction;
            dying = true;
        }
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
            dead = true;
        }
        UpdateAnimation();
    }
}
