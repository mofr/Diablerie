using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour
{

	void LateUpdate () {
        if (PlayerController.instance.character == null)
            return;

        transform.position = CalcTargetPos();
	}

	Vector3 CalcTargetPos() {
		Vector3 targetPos = PlayerController.instance.character.transform.position;
		targetPos.z = transform.position.z;

		return targetPos;
	}
}