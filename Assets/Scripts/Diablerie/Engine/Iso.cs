using Diablerie.Engine.Utility;
using UnityEngine;

namespace Diablerie.Engine
{
    [ExecuteInEditMode]
    public class Iso : MonoBehaviour
    {
        public const int SubTileCount = 5;
        public const float pixelsPerUnit = 80;
        public const float tileSize = 0.2f;
        public const float tileSizeY = tileSize / 2;
        public Vector2 pos;
        public bool macro = false;
        public bool sort = true;

        new Renderer renderer;

        public static Vector3 MapToWorld(float x, float y)
        {
            return new Vector3(x - y, -(x + y) / 2) * tileSize;
        }

        public static Vector3 MapToWorld(Vector3 iso)
        {
            return MapToWorld(iso.x, iso.y);
        }

        public static Vector3 MapTileToWorld(Vector2 iso)
        {
            return MapToWorld(iso) / tileSize;
        }

        public static Vector3 MapTileToWorld(int x, int y)
        {
            return MapTileToWorld(new Vector3(x, y));
        }

        public static Vector3 MapToIso(Vector3 world)
        {
            return new Vector3(-world.y + world.x / 2, -world.y - world.x / 2) / tileSize;
        }

        public static int SortingOrder(Vector3 worldPosition)
        {
            var macroTile = MacroTile(MapToIso(worldPosition));
            var macroY = (MapToWorld(macroTile)).y / tileSizeY;
            int macroTileOrder = -Mathf.RoundToInt(macroY);
            int sortingOrder = -Mathf.RoundToInt(worldPosition.y / tileSizeY - macroY);
            sortingOrder += macroTileOrder * 100;
            return sortingOrder;
        }

        public static void DebugDrawTile(Vector3 pos, Color color, float margin = 0, float duration = 0f)
        {
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

        public static void DebugDrawLine(Vector3 from, Vector3 to)
        {
            Debug.DrawLine(MapToWorld(from), MapToWorld(to));
        }

        public static void GizmosDrawTile(Vector3 pos, float size = 1.0f)
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

        public static void DebugDrawTile(Vector3 pos, float margin = 0, float duration = 0f)
        {
            DebugDrawTile(pos, Color.white, margin, duration);
        }

        public static Vector2i Snap(Vector3 pos)
        {
            return new Vector2i(
                Mathf.RoundToInt(pos.x),
                Mathf.RoundToInt(pos.y));
        }

        public static Vector3 MacroTile(Vector3 pos)
        {
            var macroTile = pos;
            macroTile.x = Mathf.Round(pos.x / 5);
            macroTile.y = Mathf.Round(pos.y / 5);
            return macroTile;
        }

        public static int Direction(Vector2 from, Vector3 target, int directionCount)
        {
            var dir = target - (Vector3)from;
            var angle = Vector3.Angle(new Vector3(1, 1), dir) * Mathf.Sign(dir.y - dir.x);
            var directionDegrees = 360.0f / directionCount;
            return Mathf.RoundToInt((angle + 360) % 360 / directionDegrees) % directionCount;
        }

        public static Vector2[] CreateTileRectPoints(int width, int height)
        {
            var offset = MapToWorld(-2, -2);
            return new Vector2[] {
                MapTileToWorld(0, 0) + offset,
                MapTileToWorld(width, 0) + offset,
                MapTileToWorld(width, height) + offset,
                MapTileToWorld(0, height) + offset
            };
        }

        void Awake()
        {
            pos = MapToIso(transform.position);
            renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            transform.position = MapToWorld(pos);

            if (sort && renderer)
                renderer.sortingOrder = SortingOrder(transform.position);
        }
    }
}
