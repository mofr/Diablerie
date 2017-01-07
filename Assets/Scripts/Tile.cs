using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public bool passable = true;

	int width = 5;
	int height = 5;
	Color color = new Color(1, 1, 1, 0.07f);
	Color redColor = new Color(1, 0, 0, 0.2f);

	void Start () {
		Vector3 pos = Iso.MapToIso(transform.position);
		pos.x -= width / 2;
		pos.y -= height / 2;
		for (int x = 0; x < height; ++x) {
			for (int y = 0; y < width; ++y) {
				Tilemap.instance[pos + new Vector3(x, y)] = passable;
			}
		}
	}

	void Update () {
		Vector3 pos = Iso.MapToIso(transform.position);
		pos.x -= width / 2;
		pos.y -= height / 2;
		for (int x = 0; x < height; ++x) {
			for (int y = 0; y < width; ++y) {
				Iso.DebugDrawTile(pos + new Vector3(x, y), passable ? color: redColor);
			}
		}
	}
}
