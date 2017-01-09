using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	Iso iso;
	Character character;

	void Start () {
		iso = GetComponent<Iso>();
		character = GetComponent<Character>();
	}

	void Update () {
		Vector3 targetTile;
		if (Usable.hot != null) {
			targetTile = Iso.MapToIso(Usable.hot.transform.position);
		} else {
			targetTile = Iso.MouseTile();
		}
		Iso.DebugDrawTile(targetTile, Tilemap.instance[targetTile] ? Color.green : Color.red, 0.1f);
		Pathing.BuildPath(iso.tilePos, targetTile);

		if (Input.GetMouseButton(0)) {
			if (Usable.hot != null) {
				character.Use(Usable.hot);
			} else {
				character.GoTo(targetTile);
			}
		}

		if (Input.GetMouseButtonDown(1)) {
			character.Teleport(Iso.MouseTile());
		}
	}
}
