using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder
{
    public readonly int gridX;
    public readonly int gridY;
    public readonly int gridWidth;
    public readonly int gridHeight;

    LevelInfo info;
    string name;
    DS1[] grid;
    List<Popup> popups = new List<Popup>();
    DT1.Sampler tileSampler = new DT1.Sampler();
    MonStat[] monStats;

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

    public LevelBuilder(string name, int gridX = -1, int gridY = -1)
    {
        info = LevelInfo.Find(name);
        this.name = info.levelName;

        if (info.preset != null)
        {
            var ds1 = DS1.Load(info.preset.ds1Files[0]);
            this.gridX = ds1.width - 1;
            this.gridY = ds1.height - 1;
            gridWidth = 1;
            gridHeight = 1;
            grid = new DS1[1] { ds1 };
        }
        else
        {
            this.gridX = gridX;
            this.gridY = gridY;
            gridWidth = info.sizeX / gridX;
            gridHeight = info.sizeY / gridY;
            grid = new DS1[gridWidth * gridHeight];
        }

        InitTileSampler();
    }

    public LevelBuilder(DS1 ds1)
    {
        name = System.IO.Path.GetFileName(ds1.filename);
        grid = new DS1[1] { ds1 };
        gridWidth = 1;
        gridHeight = 1;
        gridX = ds1.width - 1;
        gridY = ds1.height - 1;
    }

    private void InitTileSampler()
    {
        tileSampler = new DT1.Sampler();
        if (info != null)
        {
            foreach (var dt1Filename in info.type.dt1Files)
            {
                var dt1 = DT1.Load(dt1Filename);
                tileSampler.Add(dt1.tiles);
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

    public void Place(DS1 ds1, Vector2i pos)
    {
        Debug.Assert(ds1.width - 1 == gridX);
        Debug.Assert(ds1.height - 1 == gridY);
        grid[pos.y * gridWidth + pos.x] = ds1;
    }

    void InstantiatePopups(DS1 ds1, int offsetX, int offsetY, Transform parent = null)
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
                            var scanArea = new IntRect(offsetX, offsetY, ds1.width, ds1.height);
                            var triggerArea = new IntRect(x1 + offsetX, y1 + offsetY, x - x1, y - y1);
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
            collider.points = Iso.CreateTileRectPoints(info.sizeX, info.sizeY);
            collider.isTrigger = true;

            var level = root.AddComponent<Level>();
            level.info = info;

            SelectMonsterTypes();
        }

        int i = 0;
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x, ++i)
            {
                var ds1 = grid[i];
                int offsetX = offset.x + x * gridX;
                int offsetY = offset.y + y * gridY;
                if (ds1 != null)
                {
                    Instantiate(ds1, offsetX, offsetY, root.transform);
                    InstantiateMonsters(offsetX, offsetY, root.transform);
                }
                else if (info != null && info.drlgType == 3)
                {
                    FillGap(offset, x, y, root.transform);
                    InstantiateMonsters(offsetX, offsetY, root.transform);
                }
            }
        }

        InstantiateDebugGrid(offset, root.transform);

        return root;
    }

    private void Instantiate(DS1 ds1, int x, int y, Transform root)
    {
        InstantiatePopups(ds1, x, y, root);
        InstantiateFloors(ds1, x, y, root);
        InstantiateWalls(ds1, x, y, root);
        InstantiateObjects(ds1, x, y, root);
    }

    private void InstantiateMonsters(int offsetX, int offsetY, Transform root)
    {
        if (info == null)
            return;

        int density = info.monDen[0];

        for (int x = offsetX; x < offsetX + gridX; ++x)
        {
            for (int y = offsetY; y < offsetY + gridY; ++y)
            {
                int sample = Random.Range(0, 100000);
                if (sample >= density)
                    continue;

                var monStat = monStats[Random.Range(0, monStats.Length)];
                Spawn(monStat, x, y, root);
            }
        }
    }

    private static void Spawn(MonStat monStat, int x, int y, Transform root)
    {
        if (!CollisionMap.Passable(new Vector2i(x, y) * Iso.SubTileCount, monStat.ext.sizeX))
            return;

        int count = Random.Range(monStat.minGrp, monStat.maxGrp + 1);
        for (int i = 0; i < count; ++i)
        {
            World.SpawnMonster(monStat, Iso.MapTileToWorld(x, y), root);
        }

        if (monStat.minion1 != null)
        {
            int minionCount = Random.Range(monStat.partyMin, monStat.partyMax);
            for (int i = 0; i < minionCount; ++i)
            {
                World.SpawnMonster(monStat.minion1, Iso.MapTileToWorld(x, y), root);
            }
        }
    }

    private void SelectMonsterTypes()
    {
        if (info == null)
            return;

        monStats = new MonStat[info.numMon];
        int[] monsterColumns = new int[info.numMon];
        for (int i = 0; i < info.numMon; ++i)
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
    }

    private void InstantiateDebugGrid(Vector2i offset, Transform root)
    {
        var grid = new GameObject("debug grid");
        grid.transform.SetParent(root);
        grid.layer = UnityLayers.SpecialTiles;

        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                var cellObject = new GameObject(x + ", " + y);
                cellObject.transform.position = Iso.MapToWorld(
                    (x * gridX + offset.x) * Iso.SubTileCount - 2,
                    (y * gridY + offset.y) * Iso.SubTileCount - 2);
                cellObject.transform.SetParent(grid.transform);
                cellObject.layer = UnityLayers.SpecialTiles;
                var line = cellObject.AddComponent<LineRenderer>();
                line.startWidth = 0.1f;
                line.endWidth = 0.1f;
                line.material = Materials.normal;
                line.useWorldSpace = false;
                var corners = new Vector3[] {
                    Iso.MapTileToWorld(0, 0),
                    Iso.MapTileToWorld(0 + gridX, 0),
                    Iso.MapTileToWorld(0 + gridX, gridY),
                    Iso.MapTileToWorld(0, gridY),
                    Iso.MapTileToWorld(0, 0)
                };
                line.numPositions = corners.Length;
                line.SetPositions(corners);
            }
        }
    }

    private void InstantiateFloors(DS1 ds1, int offsetX, int offsetY, Transform root)
    {
        var layerObject = new GameObject("floors");
        var layerTransform = layerObject.transform;
        layerTransform.SetParent(root);

        for (int f = 0; f < ds1.floors.Length; ++f)
        {
            var floors = ds1.floors[f];
            int i = 0;
            for (int y = 0; y < ds1.height - 1; ++y)
            {
                for (int x = 0; x < ds1.width - 1; ++x)
                {
                    var cell = floors[i + x];
                    if (cell.prop1 == 0) // no tile here
                        continue;

                    if ((cell.prop4 & 0x80) != 0)
                        continue;

                    DT1.Tile tile;
                    if (ds1.tileSampler.Sample(cell.tileIndex, out tile))
                    {
                        CreateTile(tile, offsetX + x, offsetY + y, parent: layerTransform);
                    }
                }
                i += ds1.width;
            }
        }
    }

    private void InstantiateWalls(DS1 ds1, int offsetX, int offsetY, Transform root)
    {
        for (int w = 0; w < ds1.walls.Length; ++w)
        {
            var layerObject = new GameObject("walls " + (w + 1));
            var layerTransform = layerObject.transform;
            layerTransform.SetParent(root);
            var sampler = ds1.tileSampler;

            var cells = ds1.walls[w];
            int i = 0;
            for (int y = 0; y < ds1.height - 1; ++y)
            {
                for (int x = 0; x < ds1.width - 1; ++x)
                {
                    var cell = cells[i + x];
                    if (cell.prop1 == 0) // no tile here
                        continue;

                    DT1.Tile tile;

                    if (cell.orientation == 10 || cell.orientation == 11)
                    {
                        CreateSpecialTile(cell, offsetX + x, offsetY + y, parent: root);
                        continue;
                    }

                    if (sampler.Sample(cell.tileIndex, out tile))
                    {
                        var renderer = CreateTile(tile, offsetX + x, offsetY + y, parent: layerTransform);
                        PutToPopup(cell, renderer, offsetX + x, offsetY + y);
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
                            CreateTile(tile, offsetX + x, offsetY + y, parent: layerTransform);
                        }
                        else
                        {
                            Debug.LogWarning("wall tile not found (index " + cell.mainIndex + " " + cell.subIndex + " " + orientation + ") at " + x + ", " + y);
                        }
                    }
                }
                i += ds1.width;
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
            if (levelWarpInfo == null)
            {
                Debug.LogWarning("Warp info wasn't found");
                return;
            }
            Warp.Create(x, y, levelWarpInfo, info, targetLevel, parent);
        }
    }

    private void InstantiateObjects(DS1 ds1, int offsetX, int offsetY, Transform root)
    {
        offsetX *= Iso.SubTileCount;
        offsetY *= Iso.SubTileCount;
        foreach (var info in ds1.objects)
        {
            var gameObject = CreateObject(info.obj, offsetX + info.x, offsetY + info.y, root);
            if (gameObject == null)
                Debug.LogWarning("Object not instantiated " + info.obj.description);
        }
    }

    private void FillGap(Vector2i offset, int x, int y, Transform root)
    {
        int offsetX = x * gridX;
        int offsetY = y * gridY;

        for (y = offsetY; y < offsetY + gridY; ++y)
        {
            for (x = offsetX; x < offsetX + gridX; ++x)
            {
                DT1.Tile tile;
                tileSampler.Sample(0, out tile);
                CreateTile(tile, offset.x + x, offset.y + y, parent: root);
            }
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
        var collisionMapOffset = Iso.Snap(Iso.MapToIso(pos));
        byte mask = DT1.BlockFlags.Walk | DT1.BlockFlags.PlayerWalk;
        for (int dy = 2; dy > -3; --dy)
        {
            for (int dx = -2; dx < 3; ++dx, ++flagIndex)
            {
                var subCellPos = collisionMapOffset + new Vector2i(dx, dy);
                bool passable = (tile.flags[flagIndex] & mask) == 0;
                if (tile.orientation == 0)
                {
                    CollisionMap.SetPassable(subCellPos, passable);
                }
                else if (CollisionMap.Passable(subCellPos) && !passable)
                {
                    CollisionMap.SetPassable(subCellPos, false);
                }
            }
        }

        meshRenderer.material = tile.material;
        return meshRenderer;
    }

    static GameObject CreateObject(Obj obj, int x, int y, Transform root)
    {
        var pos = Iso.MapToWorld(x - 2, y - 2);
        if (obj.type == 2)
        {
            if (obj.objectId >= ObjectInfo.sheet.rows.Count)
            {
                return null;
            }
            ObjectInfo objectInfo = ObjectInfo.sheet.rows[obj.objectId];
            var staticObject = World.SpawnObject(objectInfo, pos, parent: root);
            staticObject.modeName = obj.mode;
            return staticObject.gameObject;
        }
        else
        {
            string monPreset = MonPreset.Find(obj.act, obj.id);
            MonStat monStat = null;
            SuperUnique superUnique = null;

            if (monPreset != null)
            {
                monStat = MonStat.Find(monPreset);
                if (monStat == null)
                    superUnique = SuperUnique.Find(monPreset);
            }
            else
            {
                monStat = MonStat.sheet.rows[obj.id];
            }

            if (monStat != null)
                return World.SpawnMonster(monStat, pos, root).gameObject;

            if (superUnique != null)
            {
                var monster = World.SpawnMonster(superUnique.monStat, pos, root);
                monster.gameObject.name = superUnique.nameStr;
                monster.title = superUnique.name;
                int minionCount = Random.Range(superUnique.minGrp, superUnique.maxGrp + 1);
                for (int i = 0; i < minionCount; ++i)
                    World.SpawnMonster(superUnique.monStat, pos, root);
                return monster.gameObject;
            }
            
            return null;
        }
    }

    public Vector2i FindEntry()
    {
        foreach (var ds1 in grid)
        {
            if (ds1 == null)
                continue;

            foreach (var cells in ds1.walls)
            {
                int i = 0;
                for (int y = 0; y < ds1.height - 1; ++y)
                {
                    for (int x = 0; x < ds1.width - 1; ++x, ++i)
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
        }

        return new Vector2i(info.sizeX, info.sizeY) / 2;
    }
}
