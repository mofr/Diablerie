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
		Vector3 tilePos = Iso.Snap(iso.pos);
		Vector3 mouseTile = Iso.MouseTile();
		Iso.DebugDrawTile(mouseTile, Tilemap.instance[mouseTile] ? Color.green : Color.red, 0.1f);
		Pathing.DebugDrawPath(tilePos, Pathing.BuildPath(tilePos, mouseTile));

		if (Input.GetMouseButton(0)) {
			character.GoTo(Iso.MouseTile());
		}

		if (Input.GetMouseButtonDown(1)) {
			character.Teleport(Iso.MouseTile());
		}
	}
}
