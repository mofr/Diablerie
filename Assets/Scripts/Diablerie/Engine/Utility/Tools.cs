using UnityEngine;

namespace Diablerie.Engine.Utility
{
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

        static public float ManhattanDistance(Vector2 a, Vector2 b)
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

        static Vector3[] rectTransformCorners = new Vector3[4];

        static public Rect RectTransformToScreenRect(RectTransform transform)
        {
            transform.GetWorldCorners(rectTransformCorners);
            var pos0 = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransformCorners[0]);
            var pos1 = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransformCorners[1]);
            var pos2 = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransformCorners[2]);
            var pos3 = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransformCorners[3]);
            return new Rect(pos0.x, pos0.y, pos2.x - pos1.x, pos1.y - pos0.y);
        }
    }
}
