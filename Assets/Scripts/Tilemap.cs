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
            pos.x += tile.offsetX;
            pos.y += tile.offsetY;
			for (int x = 0; x < tile.width; ++x) {
				for (int y = 0; y < tile.height; ++y) {
					Tilemap.instance[pos + new Vector3(x, y)] = tile.passable;
				}
			}
		}
	}

	void Update() {
		Color color = new Color(1, 1, 1, 0.15f);
		Color redColor = new Color(1, 0, 0, 0.3f);
		Vector3 pos = Iso.Snap(Iso.MapToIso(Camera.main.transform.position));
		int debugWidth = 100;
		int debugHeight = 100;
		pos.x -= debugWidth / 2;
		pos.y -= debugHeight / 2;
		for (int x = 0; x < debugWidth; ++x) {
			for (int y = 0; y < debugHeight; ++y) {
				Iso.DebugDrawTile(pos + new Vector3(x, y), this[pos + new Vector3(x, y)] ? color: redColor, 0.9f);
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

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.35f, 0.35f, 0.35f);
        for (int x = -10; x < 10; ++x)
        {
            for (int y = -10; y < 10; ++y)
            {
                var pos = Iso.MapToWorld(new Vector3(x, y) - new Vector3(0.5f, 0.5f)) / Iso.tileSize;
                Gizmos.DrawRay(pos, new Vector3(20, 10));
                Gizmos.DrawRay(pos, new Vector3(20, -10));
            }
        }
    }
}
