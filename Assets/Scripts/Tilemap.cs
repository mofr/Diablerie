using System;
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

	class TileOrderComparer : IComparer<Tile> {
		public int Compare(Tile a, Tile b) {
			bool floor1 = a.GetComponent<SpriteRenderer>().sortingLayerName == "Floor";
			bool floor2 = b.GetComponent<SpriteRenderer>().sortingLayerName == "Floor";
			return -floor1.CompareTo(floor2);
		}
	}

	void Start() {
		Tile[] tiles = GameObject.FindObjectsOfType<Tile>();
		Array.Sort(tiles, new TileOrderComparer());
		foreach (Tile tile in tiles) {
			Vector3 pos = Iso.MapToIso(tile.transform.position);
			pos.x -= tile.width / 2;
			pos.y -= tile.height / 2;
			for (int x = 0; x < tile.height; ++x) {
				for (int y = 0; y < tile.width; ++y) {
					Tilemap.instance[pos + new Vector3(x, y)] = tile.passable;
				}
			}
		}
	}

	void Update() {
		Color color = new Color(1, 1, 1, 0.07f);
		Color redColor = new Color(1, 0, 0, 0.2f);
		Vector3 pos = Iso.Snap(Iso.MapToIso(Camera.main.transform.position));
		int debugWidth = 100;
		int debugHeight = 100;
		pos.x -= debugWidth / 2;
		pos.y -= debugHeight / 2;
		for (int x = 0; x < debugWidth; ++x) {
			for (int y = 0; y < debugHeight; ++y) {
				Iso.DebugDrawTile(pos + new Vector3(x, y), this[pos + new Vector3(x, y)] ? color: redColor);
			}
		}
	}

	private int MapToIndex(Vector3 tilePos) {
		return origin + (int)Mathf.Round(tilePos.x + tilePos.y * width);
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
