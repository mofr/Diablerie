using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTools : MonoBehaviour {

	static float fmod(float a, float b) {
		return a - b * Mathf.Round(a / b);
	}

	[MenuItem("Iso/Snap")]
	static public void SnapToIsoGrid() {
		foreach (GameObject gameObject in Selection.gameObjects) {
			Snap(gameObject.transform);
		}
	}

	static public void Snap(Transform transform) {
		var pos = transform.localPosition;
		transform.localPosition = new Vector3(pos.x - fmod(pos.x, 0.2f), pos.y - fmod(pos.y, 0.1f), pos.z);
		for (int i = 0; i < transform.childCount; ++i) {
			Snap(transform.GetChild(i));
		}
	}
}
