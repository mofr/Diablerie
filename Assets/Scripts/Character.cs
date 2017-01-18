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
	Animator animator;
	List<Pathing.Step> path = new List<Pathing.Step>();
	float traveled = 0;
	int targetDirection = 0;
	bool attack = false;
	int attackAnimation;
    GameObject m_Target;
    Usable usable;
    Character targetCharacter;

    void Start() {
		iso = GetComponent<Iso>();
		animator = GetComponent<Animator>();
	}

    void PathTo(Vector2 target, float minRange = 0.1f)
    {
        AbortMovement();
        Vector2 startPos = path.Count > 0 ? path[0].pos : iso.tilePos;
        path.AddRange(Pathing.BuildPath(Iso.Snap(startPos), target, directionCount, minRange));
    }

	public void Use(Usable usable) {
        if (attack)
            return;
        PathTo(usable.GetComponent<Iso>().tilePos, useRange);
		this.usable = usable;
	}

	public void GoTo(Vector2 target) {
		if (attack)
			return;

        PathTo(target);
	}

	public void Teleport(Vector2 target) {
		if (attack)
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
            }
            if (targetCharacter)
            {
                Vector2 target = targetCharacter.GetComponent<Iso>().tilePos;
                if (Vector2.Distance(target, iso.tilePos) <= attackRange)
                {
                    attack = true;
                    attackAnimation = Random.Range(1, 3);
                    direction = Iso.Direction(iso.tilePos, target, directionCount);
                }
                targetCharacter = null;
            }
            m_Target = null;
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
		bool preserveTime = false;
		string animation;
		animator.speed = 1.0f;
		if (attack) {
			animation = "Attack" + attackAnimation;
			animator.speed = attackSpeed;
		} else if (path.Count == 0) {
			animation = "Idle";
			preserveTime = true;
		} else {
			animation = run ? "Run" : "Walk";
			preserveTime = true;
			targetDirection = path[0].directionIndex;
		}

		if (!attack && direction != targetDirection) {
			int diff = (int)Mathf.Sign(Tools.ShortestDelta(direction, targetDirection, directionCount));
			direction = (direction + diff + directionCount) % directionCount;
		}

		animation += "_" + direction.ToString();
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animation)) {
            if (preserveTime)
				animator.Play(animation, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
			else
				animator.Play(animation);
		}
	}

	public void LookAt(Vector3 target)
	{
        targetDirection = Iso.Direction(iso.tilePos, target, directionCount);
    }

    public void Attack() {
		if (!attack && direction == targetDirection && path.Count == 0) {
			attack = true;
            attackAnimation = Random.Range(1, 3);
		}
	}

    public void Attack(Character targetCharacter)
    {
        if (attack)
            return;

        Iso targetIso = targetCharacter.GetComponent<Iso>();
        PathTo(targetIso.tilePos, attackRange);
        this.targetCharacter = targetCharacter;
    }

	void OnAnimationFinish() {
        if (attack)
			attack = false;
	}

	void OnAttack1Finish() {
	}

	void OnAttack2Finish() {
    }
}
