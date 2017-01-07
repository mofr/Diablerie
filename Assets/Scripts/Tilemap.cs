using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilemap : MonoBehaviour {

	static public Tilemap instance;

	private int width = 1024;
	private int height = 1024;
	private int origin;
	private bool[] map;

	void Awake() {
		map = new bool[width * height];
		origin = map.Length / 2;
		instance = this;
	}

	void Start() {
		for (int i = 0; i < map.Length; ++i) {
			map[i] = false;
		}
	}

	void Update() {
//		Color color = new Color(1, 1, 1, 0.07f);
//		Color redColor = new Color(1, 0, 0, 0.2f);
//		Vector3 pos = Iso.MapToIso(transform.position);
//		pos.x -= 50;
//		pos.y -= 50;
//		for (int x = 0; x < 100; ++x) {
//			for (int y = 0; y < 100; ++y) {
//				Iso.DebugDrawTile(pos + new Vector3(x, y), this[pos + new Vector3(x, y)] ? color: redColor);
//			}
//		}
	}

	private int MapToIndex(Vector3 tilePos) {
		return origin + (int)(tilePos.x + tilePos.y * width);
	}

	public bool this[Vector3 tilePos]
	{
		get {
			return map[MapToIndex(tilePos)];
		}

		set {
			map[MapToIndex(tilePos)] = value;
		}
	}
}
