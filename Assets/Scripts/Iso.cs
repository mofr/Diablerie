using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
[RequireComponent (typeof(SpriteRenderer))]
public class Iso : MonoBehaviour {

    public const float pixelsPerUnit = 80;
    public const float tileSize = 0.2f;
    public const float tileSizeY = tileSize / 2;
    public Vector2 pos;
    public bool macro = false;
	public bool sort = true;

	SpriteRenderer spriteRenderer;

	static public Vector3 MapToWorld(Vector3 iso) {
		return new Vector3(iso.x - iso.y, (iso.x + iso.y) / 2) * tileSize;
	}

	static public Vector3 MapToIso(Vector3 world) {
		return new Vector3(world.y + world.x / 2, world.y - world.x / 2) / tileSize;
	}

    static public int SortingOrder(Vector3 worldPosition)
    {
        int sortingOrder = -Mathf.RoundToInt(worldPosition.y / tileSizeY);
        var macroTile = MacroTile(MapToIso(worldPosition));
        int macroTileOrder = -Mathf.RoundToInt((MapToWorld(macroTile)).y / tileSizeY);
        sortingOrder += macroTileOrder * 1000;
        return sortingOrder;
    }

	static public void DebugDrawTile(Vector3 pos, Color color, float margin = 0, float duration = 0f) {
		float d = 0.5f - margin;
        var topRight = MapToWorld(pos + new Vector3(d, d));
        var topLeft = MapToWorld(pos + new Vector3(-d, d));
        var bottomRight = MapToWorld(pos + new Vector3(d, -d));
        var bottomLeft = MapToWorld(pos + new Vector3(-d, -d));
        Debug.DrawLine(topRight, bottomRight, color, duration);
		Debug.DrawLine(bottomLeft, topLeft, color, duration);
		Debug.DrawLine(topRight, topLeft, color, duration);
		Debug.DrawLine(bottomRight, bottomLeft, color, duration);
	}

    static public void GizmosDrawTile(Vector3 pos, float size = 1.0f)
    {
        float d = 0.5f * size;
        var topRight = MapToWorld(pos + new Vector3(d, d));
        var topLeft = MapToWorld(pos + new Vector3(-d, d));
        var bottomRight = MapToWorld(pos + new Vector3(d, -d));
        var bottomLeft = MapToWorld(pos + new Vector3(-d, -d));
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }

    static public void DebugDrawTile(Vector3 pos, float margin = 0, float duration = 0f) {
		DebugDrawTile(pos, Color.white, margin, duration);
	}

	static public Vector3 Snap(Vector3 pos) {
		pos.x = Mathf.Round(pos.x);
		pos.y = Mathf.Round(pos.y);
		return pos;
	}

    static public Vector3 MacroTile(Vector3 pos)
    {
        var macroTile = pos;
        macroTile.x = Mathf.Round(pos.x / 5);
        macroTile.y = Mathf.Round(pos.y / 5);
        return macroTile;
    }

    static public int Direction(Vector2 from, Vector3 target, int directionCount)
    {
        var dir = target - (Vector3)from;
        var angle = Vector3.Angle(new Vector3(-1, -1), dir) * Mathf.Sign(dir.y - dir.x);
        var directionDegrees = 360.0f / directionCount;
        return Mathf.RoundToInt((angle + 360) % 360 / directionDegrees) % directionCount;
    }

	void Awake() {
		pos = MapToIso(transform.position);
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Start () {
		
	}

    void Update () {
        if (Application.isPlaying)
        {
            transform.position = MapToWorld(pos);
        }
        else
        {
            if (macro)
            {
                transform.position = MapToWorld(MacroTile(MapToIso(transform.position))) * 5;
            }
            else
            {
                transform.position = MapToWorld(Snap(MapToIso(transform.position)));
            }
            pos = MapToIso(transform.position);
        }

		if (sort) {
			spriteRenderer.sortingOrder = SortingOrder(transform.position);
		}
    }
}
