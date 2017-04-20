using UnityEngine;

public class Tools
{
    static public float Mod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    static public float ShortestDelta(float a, float b, float range)
    {
        return Mod(b - a + range / 2, range) - range / 2;
    }

    static public float manhattanDistance(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    static public void DebugDrawBounds(Bounds bounds)
    {
        Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y), new Vector3(bounds.max.x, bounds.min.y));
        Debug.DrawLine(new Vector3(bounds.max.x, bounds.max.y), new Vector3(bounds.max.x, bounds.min.y));
        Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y), new Vector3(bounds.max.x, bounds.max.y));
        Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y), new Vector3(bounds.min.x, bounds.min.y));
    }
}
