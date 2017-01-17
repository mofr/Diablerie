using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	public int directionCount = 8;
	public float speed = 3.5f;
	public bool run = false;

	[HideInInspector]
	public Usable usable;

	[HideInInspector]
	public int direction = 0;

	Iso iso;
	Animator animator;
	List<Pathing.Step> path = new List<Pathing.Step>();
	float traveled = 0;
	int targetDirection = 0;

	void Start() {
		iso = GetComponent<Iso>();
		animator = GetComponent<Animator>();
	}

	public void Use(Usable usable) {
		if (this.usable == usable)
			return;
		GoTo(usable.GetComponent<Iso>().tilePos);
		this.usable = usable;
	}

	public void GoTo(Vector2 target) {
		this.usable = null;

		Vector2 startPos = iso.tilePos;
		if (path.Count > 0) {
			var firstStep = path[0];
			startPos = firstStep.pos;
			path.Clear();
			path.Add(firstStep);
		} else {
			path.Clear();
			traveled = 0;
		}

		path.AddRange(Pathing.BuildPath(Iso.Snap(startPos), target, directionCount));
	}

	public void Teleport(Vector2 target) {
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

	void Update() {
		Iso.DebugDrawTile(iso.tilePos);
		Pathing.DebugDrawPath(path);

		Move();

		if (path.Count == 0 && usable) {
			usable.Use();
			usable = null;
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
		String animation;
		if (path.Count == 0) {
			animation = "Idle";
		}
		else {
			animation = run ? "Run" : "Walk";
			targetDirection = path[0].directionIndex;
		}

		if (direction != targetDirection) {
			int diff = (int)Mathf.Sign(Tools.ShortestDelta(direction, targetDirection, directionCount));
			direction = (direction + diff + directionCount) % directionCount;
		}

		animation += "_" + direction.ToString();
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animation)) {
			animator.Play(animation, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
	}

	public void LookAt(Vector3 target)
	{
		var dir = target - (Vector3)iso.pos;
		var angle = Vector3.Angle(new Vector3(-1, -1), dir) * Mathf.Sign(dir.y - dir.x);
		var directionDegrees = 360.0f / directionCount;
		targetDirection = Mathf.RoundToInt((angle + 360) % 360 / directionDegrees) % directionCount;
	}
}
