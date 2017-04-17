using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoInput : MonoBehaviour {

	static public Vector2 mousePosition;
	static public Vector3 mouseTile;

	void Update ()
    {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition = Iso.MapToIso(mousePos);
		mouseTile = Iso.Snap(mousePosition);
	}
}
