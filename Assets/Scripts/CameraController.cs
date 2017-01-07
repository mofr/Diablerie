using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

	public Transform target;

	void Start () {
		if (!target) {
			target = GameObject.FindWithTag("Player").transform;
		}

		transform.position = CalcTargetPos();
	}

	void LateUpdate () {
		transform.position = CalcTargetPos();
	}

	Vector3 CalcTargetPos() {
		Vector3 targetPos = target.position;
		targetPos.z = transform.position.z;

		return targetPos;
	}
}