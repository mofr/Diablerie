using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public readonly int width;
    public readonly int height;
    LevelInfo info;
    string name;
    DS1.Cell[] floors;
    List<DS1.Cell[]> walls = new List<DS1.Cell[]>();
    List<DS1.ObjectSpawnInfo> objects = new List<DS1.ObjectSpawnInfo>();
    DT1.Sampler[] samplers;
    DT1.Sampler tileSampler = new DT1.Sampler();

    static readonly int mapEntryIndex = DT1.Tile.Index(30, 11, 10);
    static readonly int townEntryIndex = DT1.Tile.Index(30, 0, 10);
    static readonly int townEntry2Index = DT1.Tile.Index(31, 0, 10);
    static readonly int corpseLocationIndex = DT1.Tile.Index(32, 0, 10);
    static readonly int portalLocationIndex = DT1.Tile.Index(33, 0, 10);

    static DT1.Sampler specialTiles = new DT1.Sampler();
    static Level()
    {
        Palette.LoadPalette(0);
        var dt1 = DT1.Load(Application.streamingAssetsPath + "/ds1edit.dt1", mpq: false);
        specialTiles.Add(dt1.tiles);
    }

    public Level(string name)
    {
        info = LevelInfo.Find(name);
        this.name = info.levelName;
        width = info.sizeX + 1;
        height = info.sizeY + 1;
        floors = new DS1.Cell[width * height];
        InitSamplers();
        if (info.preset != null)
        {
            var ds1 = DS1.Load(info.preset.ds1Files[0]);
            Place(ds1);
        }
    }

    public Level(DS1 ds1)
    {
        name = System.IO.Path.GetFileName(ds1.filename);
        width = ds1.width;
        height = ds1.height;
        floors = new DS1.Cell[width * height];
        InitSamplers();
        Place(ds1);
    }

    private void InitSamplers()
    {
        samplers = new DT1.Sampler[width * height];
        tileSampler = new DT1.Sampler();
        if (info != null)
        {
            foreach (var dt1Filename in info.type.dt1Files)
            {
                var dt1 = DT1.Load(dt1Filename);
                tileSampler.Add(dt1.tiles);
            }
            for (int i = 0; i < samplers.Length; ++i)
            {
                samplers[i] = tileSampler;
            }
        }
    }

    public void Place(DS1 ds1)
    {
        Place(ds1, new Vector2i());
    }

    public void Place(LevelPreset preset, Vector2i pos)
    {
        var ds1Filename = preset.ds1Files[Random.Range(0, preset.ds1Files.Count)];
        var ds1 = DS1.Load(ds1Filename);
        Place(ds1, pos, preset.killEdge);
    }

    public void Place(DS1 ds1, Vector2i pos, bool killEdge = false)
    {
        int stride = ds1.width;
        int srcWidth = ds1.width;
        int srcHeight = ds1.height;

        if (killEdge)
        {
            srcWidth -= 1;
            srcHeight -= 1;
        }

        for (int i = 0; i < ds1.floors.Length; ++i)
        {
            Blit(floors, ds1.floors[i], pos.x, pos.y, srcWidth, srcHeight, stride);
        }

        while (walls.Count < ds1.walls.Length)
            walls.Add(new DS1.Cell[width * height]);

        for (int i = 0; i < ds1.walls.Length; ++i)
        {
            Blit(walls[i], ds1.walls[i], pos.x, pos.y, srcWidth, srcHeight, stride);
        }

        for (int i = ds1.walls.Length; i < walls.Count; ++i)
        {
            Fill(walls[i], new DS1.Cell(), pos.x, pos.y, srcWidth, srcHeight);
        }

        Fill(samplers, ds1.tileSampler, pos.x, pos.y, srcWidth, srcHeight);

        for (int i = 0; i < ds1.objects.Length; ++i)
        {
            var obj = ds1.objects[i];
            obj.x += pos.x * Iso.SubTileCount;
            obj.y += pos.y * Iso.SubTileCount;
            objects.Add(obj);
        }
    }

    void Blit(DS1.Cell[] dst, DS1.Cell[] src, int offsetX, int offsetY, int srcWidth, int srcHeight, int stride)
    {
        int dstIndex = offsetX + offsetY * width;

        srcWidth = Mathf.Min(srcWidth, width - offsetX);
        srcHeight = Mathf.Min(srcHeight, height - offsetY);

        int i = 0;
        for (int y = 0; y < srcHeight; ++y)
        {
            for (int x = 0; x < srcWidth; ++x, ++i)
            {
                dst[dstIndex + x] = src[i];
            }
            i += (stride - srcWidth);
            dstIndex += width;
        }
    }

    void Fill<T>(T[] dst, T filler, int offsetX, int offsetY, int rectWidth, int rectHeight)
    {
        int dstIndex = offsetX + offsetY * width;

        for (int y = 0; y < rectHeight; ++y)
        {
            for (int x = 0; x < rectWidth; ++x)
            {
                dst[dstIndex + x] = filler;
            }
            dstIndex += width;
        }
    }

    public GameObject Instantiate(Vector2i offset)
    {
        var root = new GameObject(name);

        InstantiateFloors(offset, root);
        InstantiateWalls(offset, root);
        InstantiateObjects(offset, root);

        return root;
    }

    private void InstantiateFloors(Vector2i offset, GameObject root)
    {
        var layerObject = new GameObject("floors");
        var layerTransform = layerObject.transform;
        layerTransform.SetParent(root.transform);

        int i = 0;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x, ++i)
            {
                var cell = floors[i];
                var sampler = samplers[i];
                DT1.Tile tile;
                if (sampler.Sample(cell.tileIndex, out tile))
                {
                    var tileObject = CreateTile(tile, offset.x + x, offset.y + y);
                    tileObject.transform.SetParent(layerTransform);
                }
            }
        }
    }

    private void InstantiateWalls(Vector2i offset, GameObject root)
    {
        for (int w = 0; w < walls.Count; ++w)
        {
            var layerObject = new GameObject("walls " + (w + 1));
            var layerTransform = layerObject.transform;
            layerTransform.SetParent(root.transform);

            var cells = walls[w];
            int i = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x, ++i)
                {
                    var cell = cells[i];
                    if (cell.prop1 == 0) // no tile here
                        continue;

                    var sampler = samplers[i];

                    DT1.Tile tile;

                    if (cell.orientation == 10 || cell.orientation == 11)
                    {
                        if (specialTiles.Sample(cell.tileIndex, out tile))
                        {
                            var tileObject = CreateTile(tile, offset.x + x, offset.y + y);
                            tileObject.transform.SetParent(layerTransform);
                            tileObject.layer = UnityLayers.SpecialTiles;
                        }
                        continue;
                    }
                    
                    if (sampler.Sample(cell.tileIndex, out tile))
                    {
                        var tileObject = CreateTile(tile, offset.x + x, offset.y + y);
                        tileObject.transform.SetParent(layerTransform);
                    }
                    else
                    {
                        Debug.LogWarning("wall tile not found (index " + cell.mainIndex + " " + cell.subIndex + " " + cell.orientation + ") at " + x + ", " + y);
                    }

                    if (cell.orientation == 3)
                    {
                        int orientation = 4;
                        int index = DT1.Tile.Index(cell.mainIndex, cell.subIndex, orientation);
                        if (sampler.Sample(index, out tile))
                        {
                            var tileObject = CreateTile(tile, offset.x + x, offset.y + y);
                            tileObject.transform.SetParent(layerTransform);
                        }
                        else
                        {
                            Debug.LogWarning("wall tile not found (index " + cell.mainIndex + " " + cell.subIndex + " " + orientation + ") at " + x + ", " + y);
                        }
                    }
                }
            }
        }
    }

    private void InstantiateObjects(Vector2i offset, GameObject root)
    {
        offset *= Iso.SubTileCount;
        foreach (var info in objects)
        {
            var gameObject = CreateObject(info.obj, offset.x + info.x, offset.y + info.y);
            if (gameObject != null)
                gameObject.transform.SetParent(root.transform, true);
            else
                Debug.LogWarning("Object not instantiated " + info.obj.description);
        }
    }

    static GameObject CreateTile(DT1.Tile tile, int x, int y, int orderInLayer = 0)
    {
        var texture = tile.texture;
        var pos = Iso.MapTileToWorld(x, y);

        GameObject gameObject = new GameObject();
        gameObject.name = tile.mainIndex + "_" + tile.subIndex + "_" + tile.orientation;
        gameObject.transform.position = pos;
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        float x0 = tile.textureX;
        float y0 = tile.textureY;
        float w = tile.width / Iso.pixelsPerUnit;
        float h = (-tile.height) / Iso.pixelsPerUnit;
        if (tile.orientation == 0 || tile.orientation == 15)
        {
            var topLeft = new Vector3(-1f, 0.5f);
            if (tile.orientation == 15)
                topLeft.y += tile.roofHeight / Iso.pixelsPerUnit;
            mesh.vertices = new Vector3[] {
                topLeft,
                topLeft + new Vector3(0, -h),
                topLeft + new Vector3(w, -h),
                topLeft + new Vector3(w, 0)
            };
            mesh.triangles = new int[] { 2, 1, 0, 3, 2, 0 };
            mesh.uv = new Vector2[] {
                new Vector2 (x0 / texture.width, -y0 / texture.height),
                new Vector2 (x0 / texture.width, (-y0 +tile.height) / texture.height),
                new Vector2 ((x0 + tile.width) / texture.width, (-y0 +tile.height) / texture.height),
                new Vector2 ((x0 + tile.width) / texture.width, -y0 / texture.height)
            };

            meshRenderer.sortingLayerName = tile.orientation == 0 ? "Floor" : "Roof";
            meshRenderer.sortingOrder = orderInLayer;
        }
        else
        {
            var topLeft = new Vector3(-1f, h - 0.5f);
            mesh.vertices = new Vector3[] {
                topLeft,
                topLeft + new Vector3(0, -h),
                topLeft + new Vector3(w, -h),
                topLeft + new Vector3(w, 0)
            };
            mesh.triangles = new int[] { 2, 1, 0, 3, 2, 0 };
            mesh.uv = new Vector2[] {
                new Vector2 (x0 / texture.width, (-y0 - tile.height) / texture.height),
                new Vector2 (x0 / texture.width, -y0 / texture.height),
                new Vector2 ((x0 + tile.width) / texture.width, -y0 / texture.height),
                new Vector2 ((x0 + tile.width) / texture.width, (-y0 - tile.height) / texture.height)
            };
            meshRenderer.sortingOrder = Iso.SortingOrder(pos) - 4;
        }
        meshFilter.mesh = mesh;

        if (Application.isPlaying)
        {
            int flagIndex = 0;
            for (int dy = 2; dy > -3; --dy)
            {
                for (int dx = -2; dx < 3; ++dx)
                {
                    if ((tile.flags[flagIndex] & (1 + 8)) != 0)
                    {
                        var subCellPos = Iso.MapToIso(pos) + new Vector3(dx, dy);
                        CollisionMap.SetPassable(subCellPos, false);
                    }
                    ++flagIndex;
                }
            }
        }

        meshRenderer.material = tile.material;
        return gameObject;
    }

    static GameObject CreateObject(Obj obj, int x, int y)
    {
        var pos = Iso.MapToWorld(x - 2, y - 2);
        if (obj.type == 2)
        {
            if (obj.objectId >= ObjectInfo.sheet.rows.Count)
            {
                return null;
            }
            ObjectInfo objectInfo = ObjectInfo.sheet.rows[obj.objectId];
            var staticObject = World.SpawnObject(objectInfo, pos);
            staticObject.modeName = obj.mode;
            return staticObject.gameObject;
        }
        else
        {
            var monStat = MonStat.Find(obj.act, obj.id);
            if (monStat == null)
                return null;
            return World.SpawnMonster(monStat, pos).gameObject;
        }
    }

    public Vector2i FindEntry()
    {
        foreach(var cells in walls)
        {
            int i = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x, ++i)
                {
                    var cell = cells[i];
                    if (cell.tileIndex == mapEntryIndex)
                    {
                        return new Vector2i(x, y);
                    }
                    else if (cell.tileIndex == townEntryIndex)
                    {
                        return new Vector2i(x, y);
                    }
                    else if (cell.tileIndex == townEntry2Index)
                    {
                        return new Vector2i(x, y);
                    }
                }
            }
        }

        return new Vector2i(width, height) / 2;
    }
}
