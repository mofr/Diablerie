using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools {
	static public float Mod(float a, float b) {
		return a - b * Mathf.Floor(a / b);
	}

	static public float ShortestDelta(float a, float b, float range) {
		return Mod(b - a + range / 2, range) - range / 2;
	}
}
