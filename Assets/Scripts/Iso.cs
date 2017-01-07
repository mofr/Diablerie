using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iso : MonoBehaviour {

	static public float tileSize = 0.2f;
	public Vector2 pos;
	public Vector2 tilePos;

	static public Vector3 MapToWorld(Vector3 iso) {
		return new Vector3(iso.x - iso.y, (iso.x + iso.y) / 2) * tileSize;
	}

	static public Vector3 MapToIso(Vector3 world) {
		return new Vector3(world.y + world.x / 2, world.y - world.x / 2) / tileSize;
	}

	static public void DebugDrawTile(Vector3 pos, Color color, float margin = 0) {
		pos = Iso.MapToWorld(pos);
		float d = 0.5f - margin;
		Debug.DrawLine(pos + Iso.MapToWorld(new Vector2(d, d)), pos + Iso.MapToWorld(new Vector2(d, -d)), color);
		Debug.DrawLine(pos + Iso.MapToWorld(new Vector2(-d, -d)), pos + Iso.MapToWorld(new Vector2(-d, d)), color);
		Debug.DrawLine(pos + Iso.MapToWorld(new Vector2(d, d)), pos + Iso.MapToWorld(new Vector2(-d, d)), color);
		Debug.DrawLine(pos + Iso.MapToWorld(new Vector2(d, -d)), pos + Iso.MapToWorld(new Vector2(-d, -d)), color);
	}

	static public void DebugDrawTile(Vector3 pos, float margin = 0) {
		DebugDrawTile(pos, Color.white, margin);
	}

	static public Vector3 MouseTile() {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePos.z = 0;
		return Snap(MapToIso(mousePos));
	}

	static public Vector3 Snap(Vector3 pos) {
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);
		return pos;
	}

	void Start () {
		
	}

	void Update () {
		transform.position = MapToWorld(pos);
	}
		
	void OnDrawGizmosSelected() {
		DebugDrawTile(pos);
	}
}
