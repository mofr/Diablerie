using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

	PlayerController playerController;

	void Awake () {
		playerController = GameObject.FindObjectOfType<PlayerController>();
		transform.position = CalcTargetPos();
	}

	void LateUpdate () {
		transform.position = CalcTargetPos();
	}

	Vector3 CalcTargetPos() {
		Vector3 targetPos = playerController.character.transform.position;
		targetPos.z = transform.position.z;

		return targetPos;
	}
}