using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder
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
    List<Popup> popups = new List<Popup>();

    static readonly int mapEntryIndex = DT1.Tile.Index(30, 11, 10);
    static readonly int townEntryIndex = DT1.Tile.Index(30, 0, 10);
    static readonly int townEntry2Index = DT1.Tile.Index(31, 0, 10);
    static readonly int corpseLocationIndex = DT1.Tile.Index(32, 0, 10);
    static readonly int portalLocationIndex = DT1.Tile.Index(33, 0, 10);

    static DT1.Sampler specialTiles = new DT1.Sampler();
    static LevelBuilder()
    {
        Palette.LoadPalette(0);
        var dt1 = DT1.Load(Application.streamingAssetsPath + "/ds1edit.dt1", mpq: false);
        specialTiles.Add(dt1.tiles);
    }

    public LevelBuilder(string name)
    {
        info = LevelInfo.Find(name);
        this.name = info.levelName;
        width = info.sizeX;
        height = info.sizeY;
        floors = new DS1.Cell[width * height];
        InitSamplers();
        if (info.preset != null)
        {
            var ds1 = DS1.Load(info.preset.ds1Files[0]);
            Place(ds1, new Vector2i());
        }
    }

    public LevelBuilder(DS1 ds1)
    {
        name = System.IO.Path.GetFileName(ds1.filename);
        width = ds1.width;
        height = ds1.height;
        floors = new DS1.Cell[width * height];
        InitSamplers();
        Place(ds1, new Vector2i(), killEdge: false);
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

    public void Place(LevelPreset preset, Vector2i pos, int minIndex = 0, int maxIndex = -1)
    {
        if (maxIndex == -1)
            maxIndex = preset.ds1Files.Count;
        var ds1Filename = preset.ds1Files[Random.Range(minIndex, maxIndex)];
        var ds1 = DS1.Load(ds1Filename);
        Place(ds1, pos);
    }

    public void Place(DS1 ds1, Vector2i pos, bool killEdge = true)
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

        CreatePopups(ds1, pos);
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

    void CreatePopups(DS1 ds1, Vector2i pos, Transform parent = null)
    {
        bool firstFound = false;
        int x1 = 0;
        int y1 = 0;

        for (int layerIndex = 0; layerIndex < ds1.walls.Length; ++layerIndex)
        {
            var walls = ds1.walls[layerIndex];
            int i = 0;
            for (int y = 0; y < ds1.height; ++y)
            {
                for (int x = 0; x < ds1.width; ++x, ++i)
                {
                    if (walls[i].mainIndex == 8 && walls[i].orientation == 10)
                    {
                        if (firstFound)
                        {
                            var scanArea = new IntRect(pos.x, pos.y, ds1.width, ds1.height);
                            var triggerArea = new IntRect(x1 + pos.x, y1 + pos.y, x - x1, y - y1);
                            var popup = Popup.Create(triggerArea, scanArea, walls[i].subIndex);
                            popup.transform.SetParent(parent);
                            popups.Add(popup);
                            return;
                        }

                        x1 = x;
                        y1 = y;
                        firstFound = true;
                    }
                }
            }
        }
    }

    public GameObject Instantiate(Vector2i offset)
    {
        var root = new GameObject(name);

        if (info != null)
        {
            var collider = root.AddComponent<PolygonCollider2D>();
            collider.offset = Iso.MapTileToWorld(offset);
            collider.points = Iso.CreateTileRectPoints(width, height);
            collider.isTrigger = true;

            var level = root.AddComponent<Level>();
            level.info = info;
        }

        InstantiateFloors(offset, root);
        InstantiateWalls(offset, root);
        InstantiateObjects(offset, root);
        InstantiateMonsters(offset, root);
        InstantiateDebugGrid(offset, root, gridSize: 8);

        foreach (var popup in popups)
            popup.transform.SetParent(root.transform);

        return root;
    }

    private void InstantiateMonsters(Vector2i offset, GameObject root)
    {
        if (info == null)
            return;

        MonStat[] monStats = new MonStat[info.numMon];
        int[] monsterColumns = new int[info.numMon];
        for(int i = 0; i < info.numMon; ++i)
            monsterColumns[i] = -1;

        for (int i = 0; i < info.numMon; ++i)
        {
            int index;
            do
            {
                index = Random.Range(0, info.monsters.Count);
            }
            while (System.Array.IndexOf(monsterColumns, index) != -1);
            monsterColumns[i] = index;
            monStats[i] = MonStat.Find(info.monsters[index]);
        }

        int density = info.monDen[0];

        for (int x = 8; x < width - 8; ++x)
        {
            for (int y = 8; y < height - 8; ++y)
            {
                int sample = Random.Range(0, 100000);
                if (sample >= density)
                    continue;

                var monStat = monStats[Random.Range(0, monStats.Length)];
                int count = Random.Range(monStat.minGrp, monStat.maxGrp + 1);
                for (int i = 0; i < count; ++i)
                {
                    var monster = World.SpawnMonster(monStat, Iso.MapTileToWorld(x, y));
                    monster.transform.SetParent(root.transform);
                }
            }
        }
    }

    private void InstantiateDebugGrid(Vector2i offset, GameObject root, int gridSize)
    {
        var grid = new GameObject();
        grid.transform.SetParent(root.transform);
        grid.layer = UnityLayers.SpecialTiles;

        for (int y = 0; y < height / gridSize; ++y)
        {
            for (int x = 0; x < width / gridSize; ++x)
            {
                var cellObject = new GameObject(x + ", " + y);
                cellObject.transform.position = Iso.MapToWorld(
                    (x * gridSize + offset.x) * Iso.SubTileCount - 2,
                    (y * gridSize + offset.y) * Iso.SubTileCount - 2);
                cellObject.transform.SetParent(grid.transform);
                cellObject.layer = UnityLayers.SpecialTiles;
                var line = cellObject.AddComponent<LineRenderer>();
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                line.material = Materials.normal;
                line.useWorldSpace = false;
                var corners = new Vector3[] {
                    Iso.MapTileToWorld(0, 0) * gridSize,
                    Iso.MapTileToWorld(0 + 1, 0) * gridSize,
                    Iso.MapTileToWorld(0 + 1, 0 + 1) * gridSize,
                    Iso.MapTileToWorld(0, 0 + 1) * gridSize,
                    Iso.MapTileToWorld(0, 0) * gridSize
                };
                line.numPositions = corners.Length;
                line.SetPositions(corners);
            }
        }
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
                    CreateTile(tile, offset.x + x, offset.y + y, parent: layerTransform);
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
                        CreateSpecialTile(cell, offset.x + x, offset.y + y, parent: root.transform);
                        continue;
                    }

                    if (sampler.Sample(cell.tileIndex, out tile))
                    {
                        var renderer = CreateTile(tile, offset.x + x, offset.y + y, parent: layerTransform);
                        PutToPopup(cell, renderer, offset.x + x, offset.y + y);
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
                            var renderer = CreateTile(tile, offset.x + x, offset.y + y, parent: layerTransform);
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

    private void PutToPopup(DS1.Cell cell, Renderer renderer, int x, int y)
    {
        Popup popup = null;
        foreach (Popup iter in popups)
        {
            if ((cell.orientation != 15 || iter.tileIndex == cell.mainIndex) && iter.scanArea.Contains(x, y))
            {
                popup = iter;
                break;
            }
        }

        if (popup == null)
            return;

        if (cell.orientation == 15)
            popup.roofs.Add(renderer);
        else if (cell.orientation == 5 || cell.orientation == 6 || 
            (x != popup.triggerArea.xMin && y != popup.triggerArea.yMax))
            popup.walls.Add(renderer);
    }

    private void CreateSpecialTile(DS1.Cell cell, int x, int y, Transform parent)
    {
        // debug visualization
        DT1.Tile tile;
        if (specialTiles.Sample(cell.tileIndex, out tile))
        {
            var renderer = CreateTile(tile, x, y, parent: parent);
            renderer.gameObject.layer = UnityLayers.SpecialTiles;
        }

        if (info == null)
            return;

        if (cell.mainIndex < 8)
        {
            int targetLevelId = info.vis[cell.mainIndex];
            int warpId = info.warp[cell.mainIndex];
            var targetLevel = LevelInfo.Find(targetLevelId);
            var levelWarpInfo = LevelWarpInfo.Find(warpId);
            Warp.Create(x, y, levelWarpInfo, targetLevel, parent);
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

    static Renderer CreateTile(DT1.Tile tile, int x, int y, int orderInLayer = 0, Transform parent = null)
    {
        var texture = tile.texture;
        var pos = Iso.MapTileToWorld(x, y);

        GameObject gameObject = new GameObject();
        gameObject.name = tile.mainIndex + "_" + tile.subIndex + "_" + tile.orientation;
        gameObject.transform.position = pos;
        if (parent)
            gameObject.transform.SetParent(parent);
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
        
        int flagIndex = 0;
        var collisionMapIndex = Iso.Snap(Iso.MapToIso(pos));
        for (int dy = 2; dy > -3; --dy)
        {
            for (int dx = -2; dx < 3; ++dx)
            {
                if ((tile.flags[flagIndex] & (1 + 8)) != 0)
                {
                    var subCellPos = collisionMapIndex + new Vector2i(dx, dy);
                    CollisionMap.SetPassable(subCellPos, false);
                }
                ++flagIndex;
            }
        }

        meshRenderer.material = tile.material;
        return meshRenderer;
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
