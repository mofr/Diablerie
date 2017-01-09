using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	Iso iso;
	Animator animator;
	public int direction = 0;
	static public float speed = 3.5f;
	List<Vector2> path = new List<Vector2>();
	public Usable usable;
	float traveled = 0;

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
			Vector2 firstStep = path[0];
			startPos += firstStep;
			path.Clear();
			path.Add(firstStep);
		} else {
			path.Clear();
			traveled = 0;
		}

		path.AddRange(Pathing.BuildPath(Iso.Snap(startPos), target));
		UpdateAnimation();
	}

	public void Teleport(Vector2 target) {
		path.Clear();
		traveled = 0;
		iso.pos = target;
		iso.tilePos = target;
		UpdateAnimation();
	}

	void Update() {
		Iso.DebugDrawTile(iso.tilePos);
		Pathing.DebugDrawPath(iso.tilePos, path);

		Move();

		if (path.Count == 0 && usable) {
			usable.Use();
			usable = null;
		}
	}

	void Move() {
		if (path.Count == 0)
			return;

		Vector2 step = path[0];
		float stepLen = step.magnitude;

		float distance = speed * Time.deltaTime;
		while (traveled + distance >= stepLen) {
			float firstPart = stepLen - traveled;
			iso.pos += step.normalized * firstPart;
			distance -= firstPart;
			traveled += firstPart - stepLen;
			iso.tilePos += step;
			path.RemoveAt(0);
			UpdateAnimation();
			if (path.Count > 0)
				step = path[0];
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
			animation = "Walk";
			Vector2 vel = path[0];
			if (vel.x > 0 && vel.y < 0) {
				direction = 0;
			} else if (vel.x > 0 && vel.y == 0) {
				direction = 1;
			} else if (vel.x > 0 && vel.y > 0) {
				direction = 2;
			} else if (vel.x == 0 && vel.y > 0) {
				direction = 3;
			} else if (vel.x < 0 && vel.y > 0) {
				direction = 4;
			} else if (vel.x < 0 && vel.y == 0) {
				direction = 5;
			} else if (vel.x < 0 && vel.y < 0) {
				direction = 6;
			} else if (vel.x == 0 && vel.y < 0) {
				direction = 7;
			}
		}

		if (direction == 0) {
			animation += "Right";
		} else if (direction == 1) {
			animation += "UpRight";
		} else if (direction == 2) {
			animation += "Up";
		} else if (direction == 3) {
			animation += "UpLeft";
		} else if (direction == 4) {
			animation += "Left";
		} else if (direction == 5) {
			animation += "DownLeft";
		} else if (direction == 6) {
			animation += "Down";
		} else if (direction == 7) {
			animation += "DownRight";
		}

		animator.Play(animation);
	}
}
