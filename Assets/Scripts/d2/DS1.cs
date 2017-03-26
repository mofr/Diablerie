using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DS1
{
    public string filename;
    public Vector3 center;
    public Vector3 entry;
    public int width;
    public int height;
    Cell[][] walls;
    Cell[][] floors;
    ObjectSpawnInfo[] objects;
    Group[] groups;

    struct ObjectSpawnInfo
    {
        public int x;
        public int y;
        public Obj obj;
    }

    struct Group
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    struct Cell
    {
        public byte prop1;
        public byte prop2;
        public byte prop3;
        public byte prop4;
        public byte orientation;
        public int mainIndex;
        public int subIndex;
        public int tileIndex;
    };

    static byte[] dirLookup = {
                  0x00, 0x01, 0x02, 0x01, 0x02, 0x03, 0x03, 0x05, 0x05, 0x06,
                  0x06, 0x07, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
                  0x0F, 0x10, 0x11, 0x12, 0x14
               };

    static readonly int mapEntryIndex = DT1.Tile.Index(30, 11, 10);
    static readonly int townEntryIndex = DT1.Tile.Index(30, 0, 10);
    static readonly int townEntry2Index = DT1.Tile.Index(31, 0, 10);
    static readonly int corpseLocationIndex = DT1.Tile.Index(32, 0, 10);
    static readonly int portalLocationIndex = DT1.Tile.Index(33, 0, 10);

    static public DS1 LoadFile(string filename)
    {
        var bytes = File.ReadAllBytes(filename);
        return Load(filename, bytes);
    }

    static public DS1 Load(string filename)
    {
        var bytes = Mpq.ReadAllBytes(filename);
        return Load(filename, bytes);
    }

    static DS1 Load(string filename, byte[] bytes)
    {
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);
        DS1 ds1 = new DS1();
        ds1.filename = filename;
        int version = reader.ReadInt32();
        ds1.width = reader.ReadInt32() + 1;
        ds1.height = reader.ReadInt32() + 1;
        ds1.center = MapToWorld(ds1.width, ds1.height) / 2;
        ds1.entry = ds1.center;

        int act = 0;
        if (version >= 8)
        {
            act = reader.ReadInt32();
            act = Mathf.Min(act, 4);
        }

        Palette.LoadPalette(act);

        int tagType = 0;
        if (version >= 10)
        {
            tagType = reader.ReadInt32();
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();

        if (version >= 3)
        {
            ReadDependencies(reader);
        }

        Debug.Log("Linked DT1 files loaded in " + sw.ElapsedMilliseconds + " ms");
        sw.Reset();
        sw.Start();

        // skip 2 dwords ?
        if ((version >= 9) && (version <= 13))
            stream.Seek(8, SeekOrigin.Current);

        int wallLayerCount = 1;
        int floorLayerCount = 1;
        int shadowLayerCount = 1;
        int tagLayerCount = 0;

        if (version >= 4)
        {
            wallLayerCount = reader.ReadInt32();

            if (version >= 16)
            {
                floorLayerCount = reader.ReadInt32();
            }
        }
        else
        {
            tagLayerCount = 1;
        }

        if ((tagType == 1) || (tagType == 2))
            tagLayerCount = 1;

        Debug.Log("layers : (2 * " + wallLayerCount + " walls) + " + floorLayerCount + " floors + " + shadowLayerCount + " shadow + " + tagLayerCount + " tag");

        int layerCount = 0;
        int[] layout = new int[14];
        if (version < 4)
        {
            layout[0] = 1; // wall 1
            layout[1] = 9; // floor 1
            layout[2] = 5; // orientation 1
            layout[3] = 12; // tag
            layout[4] = 11; // shadow
            layerCount = 5;
        }
        else
        {
            layerCount = 0;
            for (int x = 0; x < wallLayerCount; x++)
            {
                layout[layerCount++] = 1 + x; // wall x
                layout[layerCount++] = 5 + x; // orientation x
            }
            for (int x = 0; x < floorLayerCount; x++)
                layout[layerCount++] = 9 + x; // floor x
            if (shadowLayerCount != 0)
                layout[layerCount++] = 11;    // shadow
            if (tagLayerCount != 0)
                layout[layerCount++] = 12;    // tag
        }
        
        ds1.floors = new Cell[floorLayerCount][];
        for (int i = 0; i < floorLayerCount; ++i)
        {
            ds1.floors[i] = new Cell[ds1.width * ds1.height];
        }

        ds1.walls = new Cell[wallLayerCount][];
        for (int i = 0; i < wallLayerCount; ++i)
        {
            ds1.walls[i] = new Cell[ds1.width * ds1.height];
        }

        for (int n = 0; n < layerCount; n++)
        {
            int p;
            int i = 0;
            for (int y = 0; y < ds1.height; y++)
            {
                for (int x = 0; x < ds1.width; x++)
                {
                    switch (layout[n])
                    {
                        // walls
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            {
                                p = layout[n] - 1;
                                ds1.walls[p][i].prop1 = reader.ReadByte();
                                ds1.walls[p][i].prop2 = reader.ReadByte();
                                ds1.walls[p][i].prop3 = reader.ReadByte();
                                ds1.walls[p][i].prop4 = reader.ReadByte();
                                break;
                            }

                        // orientations
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            {
                                p = layout[n] - 5;
                                var cell = ds1.walls[p][i];
                                cell.orientation = reader.ReadByte();
                                if (version < 7)
                                    cell.orientation = dirLookup[cell.orientation];

                                stream.Seek(3, SeekOrigin.Current);

                                if (cell.prop1 == 0)
                                    break;

                                cell.mainIndex = (cell.prop3 >> 4) + ((cell.prop4 & 0x03) << 4);
                                cell.subIndex = cell.prop2;
                                cell.tileIndex = DT1.Tile.Index(cell.mainIndex, cell.subIndex, cell.orientation);
                                if (cell.tileIndex == mapEntryIndex)
                                {
                                    ds1.entry = MapToWorld(x, y);
                                    break;
                                }
                                else if (cell.tileIndex == townEntryIndex)
                                {
                                    ds1.entry = MapToWorld(x, y);
                                    break;
                                }
                                else if (cell.tileIndex == townEntry2Index)
                                {
                                    break;
                                }
                                else if (cell.tileIndex == corpseLocationIndex)
                                {
                                    break;
                                }
                                else if (cell.tileIndex == portalLocationIndex)
                                {
                                    break;
                                }

                                ds1.walls[p][i] = cell;
                                break;
                            }

                        // floors
                        case 9:
                        case 10:
                            {
                                p = layout[n] - 9;
                                var cell = ds1.floors[p][i];
                                cell.prop1 = reader.ReadByte();
                                cell.prop2 = reader.ReadByte();
                                cell.prop3 = reader.ReadByte();
                                cell.prop4 = reader.ReadByte();

                                cell.mainIndex = (cell.prop3 >> 4) + ((cell.prop4 & 0x03) << 4);
                                cell.subIndex = cell.prop2;
                                cell.orientation = 0;
                                cell.tileIndex = DT1.Tile.Index(cell.mainIndex, cell.subIndex, cell.orientation);

                                ds1.floors[p][i] = cell;
                                break;
                            }

                        // shadow
                        case 11:
                            reader.ReadInt32();
                            break;

                        // tag
                        case 12:
                            reader.ReadInt32();
                            break;
                    }
                    ++i;
                }
            }
        }

        if (version >= 2)
        {
            int objectCount = reader.ReadInt32();
            Debug.Log("Objects " + objectCount);
            ds1.objects = new ObjectSpawnInfo[objectCount];

            for (int i = 0; i < objectCount; i++)
            {
                var info = new ObjectSpawnInfo();
                int type = reader.ReadInt32();
                int id = reader.ReadInt32();
                info.x = reader.ReadInt32();
                info.y = reader.ReadInt32();

                if (version > 5)
                {
                    reader.ReadInt32(); // flags
                }

                info.obj = Obj.Find(act, type, id);
                ds1.objects[i] = info;
            }
        }

        if (version >= 12 && (tagType == 1 || tagType == 2))
        {
            if (version >= 18)
                reader.ReadInt32();

            int groupCount = reader.ReadInt32();
            Debug.Log("Groups " + groupCount);
            ds1.groups = new Group[groupCount];

            for (int i = 0; i < groupCount; i++)
            {
                var group = new Group();
                group.x = reader.ReadInt32();
                group.y = reader.ReadInt32();
                group.width = reader.ReadInt32();
                group.height = reader.ReadInt32();
                if (version >= 13)
                {
                    reader.ReadInt32(); // unknown
                }
                ds1.groups[i] = group;
            }
        }

        sw.Stop();
        Debug.Log("DS1 loaded in " + sw.ElapsedMilliseconds + " ms");

        return ds1;
    }

    static void ReadDependencies(BinaryReader reader)
    {
        int fileCount = reader.ReadInt32();

        for (int i = 0; i < fileCount; i++)
        {
            string dependency = "";
            char c;
            while ((c = reader.ReadChar()) != 0)
            {
                dependency += c;
            }
            dependency = dependency.ToLower();
            dependency = dependency.Replace(".tg1", ".dt1");
            dependency = dependency.Replace(@"c:\d2\", "");
            dependency = dependency.Replace(@"\d2\", "");
            DT1.Load(dependency);
        }
    }

    public GameObject Instantiate()
    {
        var root = new GameObject(Path.GetFileName(filename));

        for (int f = 0; f < floors.Length; ++f)
        {
            var layerObject = new GameObject("f" + (f + 1));
            var layerTransform = layerObject.transform;
            layerTransform.SetParent(root.transform);

            var cells = floors[f];
            int i = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x, ++i)
                {
                    int prop1 = cells[i].prop1;
                    if (prop1 == 0) // no tile here
                        continue;

                    var cell = cells[i];
                    DT1.Tile tile;
                    if (DT1.Find(cell.tileIndex, out tile))
                    {
                        var tileObject = CreateTile(tile, x, y, orderInLayer: f);
                        tileObject.transform.SetParent(layerTransform);
                    }
                }
            }
        }

        for (int w = 0; w < walls.Length; ++w)
        {
            var layerObject = new GameObject("w" + (w + 1));
            var layerTransform = layerObject.transform;
            layerTransform.SetParent(root.transform);

            var cells = walls[w];
            int i = 0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x, ++i)
                {
                    int prop1 = cells[i].prop1;
                    if (prop1 == 0) // no tile here
                        continue;

                    var cell = cells[i];

                    DT1.Tile tile;
                    if (DT1.Find(cell.tileIndex, out tile))
                    {
                        var tileObject = CreateTile(tile, x, y);
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
                        if (DT1.Find(index, out tile))
                        {
                            var tileObject = CreateTile(tile, x, y);
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

        foreach (var info in objects)
        {
            var gameObject = CreateObject(info);
            if (gameObject != null)
                gameObject.transform.SetParent(root.transform);
            else
                Debug.LogWarning("Object not instantiated " + info.obj.description);
        }

        return root;
    }

    static Vector3 MapToWorld(int x, int y)
    {
        var pos = Iso.MapToWorld(new Vector3(x, y)) / Iso.tileSize;
        pos.y = -pos.y;
        return pos;
    }

    static Vector3 MapSubCellToWorld(int x, int y)
    {
        var pos = Iso.MapToWorld(new Vector3(x - 2, y - 2));
        pos.y = -pos.y;
        return pos;
    }

    static GameObject CreateTile(DT1.Tile tile, int x, int y, int orderInLayer = 0)
    {
        var texture = tile.texture;
        var pos = MapToWorld(x, y);

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
        if(tile.orientation == 0 || tile.orientation == 15)
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
            for (int dx = -2; dx < 3; ++dx)
            {
                for (int dy = 2; dy > -3; --dy)
                {
                    if ((tile.flags[flagIndex] & (1 + 8)) != 0)
                    {
                        var subCellPos = Iso.MapToIso(pos) + new Vector3(dx, dy);
                        Tilemap.SetPassable(subCellPos, false);
                    }
                    ++flagIndex;
                }
            }
        }

        meshRenderer.material = tile.material;
        return gameObject;
    }

    static GameObject CreateObject(ObjectSpawnInfo info)
    {
        var pos = MapSubCellToWorld(info.x, info.y);
        var obj = info.obj;
        if (obj.type == 2)
        {
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
}
