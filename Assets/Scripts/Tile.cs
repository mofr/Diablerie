using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Iso))]
public class Tile : MonoBehaviour {

	public bool passable = true;
	public int width = 5;
	public int height = 5;

    [Range(-5, 5)]
    public int offsetX = 0;

    [Range(-5, 5)]
    public int offsetY = 0;

    void Start () {

	}

	void Update () {
	}

    void OnDrawGizmosSelected ()
    {
        Vector3 pos = Iso.MapToIso(transform.position);
        pos.x -= width / 2;
        pos.y -= height / 2;
        pos.x += offsetX;
        pos.y += offsetY;
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                Gizmos.color = passable ? new Color(1, 1, 1, 0.2f) : new Color(1, 0, 0, 0.3f);
                Iso.GizmosDrawTile(pos + new Vector3(x, y), 0.9f);
            }
        }
    }
}
