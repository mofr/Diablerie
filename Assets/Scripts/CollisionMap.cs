using UnityEngine;

[ExecuteInEditMode]
public class CollisionMap : MonoBehaviour
{
    static private CollisionMap instance;

    public struct Cell
    {
        public bool passable;
        public GameObject gameObject;
    }

    private int width = 2048;
    private int height = 2048;
    private int origin;
    private Cell[] map;

    void OnEnable()
    {
        map = new Cell[width * height];
        origin = width * 5 + 5;
        instance = this;
    }

    private void Update()
    {
        DrawDebugCellGrid();
    }

    void DrawDebugCellGrid()
    {
        Color color = new Color(1, 0, 0, 0.3f);
        Color freeColor = new Color(1, 1, 1, 0.03f);
        Vector2i pos = Iso.Snap(Iso.MapToIso(Camera.main.transform.position));
        int debugWidth = 100;
        int debugHeight = 100;
        pos.x -= debugWidth / 2;
        pos.y -= debugHeight / 2;
        int index = instance.MapToIndex(pos);
        for (int y = 0; y < debugHeight; ++y)
        {
            for (int x = 0; x < debugWidth; ++x)
            {
                if (index + x < 0 || index + x >= instance.map.Length)
                    continue;

                if (!instance.map[index + x].passable)
                    Iso.DebugDrawTile(pos + new Vector3(x, y), color, 0.9f);
                else
                    Iso.DebugDrawTile(pos + new Vector3(x, y), freeColor, 0.9f);
            }
            index += width;
        }
    }

    private int MapToIndex(Vector3 pos)
    {
		return MapToIndex(Iso.Snap(pos));
	}

    private int MapToIndex(Vector2i pos)
    {
        return origin + pos.x + pos.y * width;
    }

    private Vector3 MapToIso(int index)
    {
        index -= origin;
        return new Vector3(index % width, index / width);
    }

    public static Cell GetCell(Vector3 pos)
    {
        var tilePos = Iso.Snap(pos);
        int index = instance.MapToIndex(tilePos);
        return instance.map[index];
    }

    public static bool Passable(Vector3 pos, int size = 1, bool debug = false, GameObject ignore = null)
    {
        return Passable(Iso.Snap(pos), size, debug, ignore);
    }

    public static bool Passable(Vector2i pos, int size = 1, bool debug = false, GameObject ignore = null)
    {
        int index = instance.MapToIndex(pos);
        return Passable(index, size, debug, ignore);
    }

    public static bool Passable(int index, int size = 1, bool debug = false, GameObject ignore = null)
    {
        UnityEngine.Profiling.Profiler.BeginSample("PassableTile");
        if (index - size - size * instance.width < 0 || index + size + size * instance.width >= instance.map.Length)
            return false;

        index = index - size / 2 - size / 2 * instance.height;
        int step = instance.width - size;
        for (int y = 0; y < size; ++y)
        {
            int end = index + size;
            while (index < end)
            {
                if (debug)
                {
                    var tilePos = instance.MapToIso(index);
                    Iso.DebugDrawTile(tilePos, 0.1f);
                }

                var cell = instance.map[index];
                if (!cell.passable && (ignore == null || ignore != cell.gameObject))
                {
                    UnityEngine.Profiling.Profiler.EndSample();
                    return false;
                }
                ++index;
            }
            index += step;
        }

        UnityEngine.Profiling.Profiler.EndSample();

        return true;
    }

    public static void SetPassable(Vector3 pos, bool passable)
    {
        SetPassable(Iso.Snap(pos), passable);
    }

    public static void SetPassable(Vector2i pos, bool passable)
    {
        int index = instance.MapToIndex(pos);
        instance.map[index].passable = passable;
    }

    public static void SetPassable(Vector3 pos, int sizeX, int sizeY, bool passable, GameObject gameObject)
    {
        SetPassable(Iso.Snap(pos), sizeX, sizeY, passable, gameObject);
    }

    public static void SetPassable(Vector2i pos, int sizeX, int sizeY, bool passable, GameObject gameObject)
    {
        int index = instance.MapToIndex(pos) - sizeX / 2 - sizeY / 2 * instance.height;
        int step = instance.width - sizeX;
        for (int y = 0; y < sizeY; ++y)
        {
            int end = index + sizeX;
            while (index < end)
            {
                Cell cell = instance.map[index];
                if (passable && cell.gameObject == gameObject)
                {
                    cell.passable = true;
                    cell.gameObject = null;
                    instance.map[index] = cell;
                }
                else if (!passable && cell.passable)
                {
                    cell.passable = false;
                    cell.gameObject = gameObject;
                    instance.map[index] = cell;
                }
                ++index;
            }
            index += step;
        }
    }

    public struct RaycastHit
    {
        public bool hit;
        public GameObject gameObject;
        public Vector2 pos;

        public static implicit operator bool(RaycastHit value)
        {
            return value.hit;
        }
    }

    static public RaycastHit Raycast(Vector2 from, Vector2 to, float rayLength = Mathf.Infinity, float maxRayLength = Mathf.Infinity, int size = 1, GameObject ignore = null, bool debug = false)
    {
        var hit = new RaycastHit();
        var diff = to - from;
        var stepLen = 0.2f;
        if (rayLength == Mathf.Infinity)
            rayLength = Mathf.Min(diff.magnitude, maxRayLength);
        int stepCount = Mathf.RoundToInt(rayLength / stepLen);
        var step = diff.normalized * stepLen;
        var pos = from;
        for (int i = 0; i < stepCount; ++i)
        {
            pos += step;
            if (debug)
                Iso.DebugDrawTile(Iso.Snap(pos), margin: 0.3f, duration: 0.5f);
            Cell cell = GetCell(pos);
            bool passable = Passable(pos, size, debug, ignore);
            if (!passable)
            {
                hit.hit = !passable;
                hit.gameObject = cell.gameObject;
                hit.pos = pos;
                break;
            }
        }
        return hit;
    }

    static public int OverlapBox(Vector2 center, Vector2 size, GameObject[] result)
    {
        int count = 0;
        if (result.Length == 0)
            return 0;
        int rows = Mathf.RoundToInt(size.y);
        int columns = Mathf.RoundToInt(size.x);
        int index = instance.MapToIndex(Iso.Snap(center - size / 2));
        for(int row = 0; row < rows; ++row)
        {
            for(int column = 0; column < columns; ++column)
            {
                var gameObject = instance.map[index + column].gameObject;
                if (gameObject != null)
                {
                    result[count] = gameObject;
                    count += 1;
                    if (count >= result.Length)
                        return count;
                }
            }
            index += instance.width;
        }
        return count;
    }

    static public void Move(Vector2 from, Vector2 to, int size, GameObject gameObject)
    {
        SetPassable(from, size, size, true, gameObject);
        SetPassable(to, size, size, false, gameObject);
    }

    static public bool Fit(Vector3 pos, out Vector3 result, int size = 1)
    {
        int index = instance.MapToIndex(pos);

        int maxIterations = 100;
        int sign = 1;
        for(int i = 1; i < maxIterations; ++i, sign=-sign)
        {
            int end = index + sign * i;
            for (; index != end && index > size && index < instance.map.Length - size - 1; index += sign)
            {
                if (Passable(index, size))
                {
                    result = instance.MapToIso(index);
                    return true;
                }
            }

            end = index - sign * i * instance.width;
            int step = -sign * instance.width;
            for (; index != end && index > size && index < instance.map.Length - size - 1; index += step)
            {
                if (Passable(index, size))
                {
                    result = instance.MapToIso(index);
                    return true;
                }
            }
        }

        result = new Vector3();
        return false;
    }
}
