using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DS1
{
    struct Cell
    {
        public byte prop1;
        public byte prop2;
        public byte prop3;
        public byte prop4;
        public byte orientation;
        public int bt_idx;
        public byte flags;
    };

    public struct ImportResult
    {
        public Vector3 center;
    }

	static public ImportResult Import(string ds1Path)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var stream = new BufferedStream(File.OpenRead(ds1Path));
        var reader = new BinaryReader(stream);
        int version = reader.ReadInt32();
        int width = reader.ReadInt32() + 1;
        int height = reader.ReadInt32() + 1;

        int act = 1;
        if (version >= 8)
        {
            act = reader.ReadInt32() + 1;
            act = Mathf.Min(act, 5);
        }

        Palette.LoadPalette(act);

        int tagType = 0;
        if (version >= 10)
        {
            tagType = reader.ReadInt32();

            //// adjust eventually the # of tag layer
            //if ((tagType == 1) || (tagType == 2))
            //    t_num = 1;
        }

        var tiles = new Dictionary<int, DT1.Tile>();

        if (version >= 3)
        {
            int fileCount = reader.ReadInt32();

            for (int i = 0; i < fileCount; i++)
            {
                string filename = "";
                char c;
                while ((c = reader.ReadChar()) != 0)
                {
                    filename += c;
                }
                filename = filename.Replace(".tg1", ".dt1");
                foreach (var tile in DT1.Import("Assets" + filename))
                {
                    if (tile.texture == null)
                        continue;
                    if (!tiles.ContainsKey(tile.index))
                        tiles[tile.index] = tile;
                }
            }
        }

        sw.Stop();
        Debug.Log("DT1 loaded in " + sw.Elapsed);
        sw.Reset();
        sw.Start();

        // skip 2 dwords ?
        if ((version >= 9) && (version <= 13))
            reader.ReadBytes(2);

        int w_num = 1; // # of wall & orientation layers
        int f_num = 1; // # of floor layer
        int s_num = 1; // # of shadow layer, always here
        int t_num = 0; // # of tag layer

        if (version >= 4)
        {
            w_num = reader.ReadInt32();

            if (version >= 16)
            {
                f_num = reader.ReadInt32();
            }
        }
        else
        {
            t_num = 1;
        }

        Debug.Log("layers : (2 * " + w_num + " walls) + " + f_num + " floors + " + s_num + " shadow + " + t_num + " tag");

        Cell[][] walls = new Cell[w_num][];
        for (int i = 0; i < f_num; ++i)
            walls[i] = new Cell[width * height];

        Cell[][] floors = new Cell[f_num][];
        for (int i = 0; i < f_num; ++i)
            floors[i] = new Cell[width * height];

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
            for (int x = 0; x < w_num; x++)
            {
                layout[layerCount++] = 1 + x; // wall x
                layout[layerCount++] = 5 + x; // orientation x
            }
            for (int x = 0; x < f_num; x++)
                layout[layerCount++] = 9 + x; // floor x
            if (s_num != 0)
                layout[layerCount++] = 11;    // shadow
            if (t_num != 0)
                layout[layerCount++] = 12;    // tag
        }

        GameObject parent = new GameObject("tristram");

        for (int n = 0; n < layerCount; n++)
        {
            int p;
            int i = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    switch (layout[n])
                    {
                        // walls
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            reader.ReadBytes(4);
                            //p = layout[n] - 1;
                            //walls[p][i].prop1 = reader.ReadByte();
                            //walls[p][i].prop2 = reader.ReadByte();
                            //walls[p][i].prop3 = reader.ReadByte();
                            //walls[p][i].prop4 = reader.ReadByte();
                            break;

                        // orientations
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            p = layout[n] - 5;
                            byte o = reader.ReadByte();
                            //if (version < 7)
                            //    walls[p][i].orientation = dir_lookup[o];
                            //else
                            //    walls[p][i].orientation = o;

                            reader.ReadBytes(3);
                            break;

                        // floors
                        case 9:
                        case 10:
                            p = layout[n] - 9;
                            int prop1 = reader.ReadByte();
                            int prop2 = reader.ReadByte();
                            int prop3 = reader.ReadByte();
                            int prop4 = reader.ReadByte();

                            int mainIndex = (prop3 >> 4) + ((prop4 & 0x03) << 4);
                            int subIndex = prop2;
                            int orientation = 0;
                            int index = (((mainIndex << 6) + subIndex) << 5) + orientation;
                            DT1.Tile tile;
                            if (tiles.TryGetValue(index, out tile))
                            {
                                var tileObject = CreateFloor(tile, orderInLayer: -p);
                                var pos = Iso.MapToWorld(new Vector3(x, y)) / Iso.tileSize;
                                pos.y = -pos.y;
                                tileObject.transform.position = pos;
                                tileObject.transform.SetParent(parent.transform);
                                break;
                            }
                            break;

                        // shadow
                        case 11:
                            reader.ReadBytes(4);
                            //if ((x < new_width) && (y < new_height))
                            //{
                            //    p = layout[n] - 11;
                            //    s_ptr[p]->prop1 = *bptr;
                            //    bptr++;
                            //    s_ptr[p]->prop2 = *bptr;
                            //    bptr++;
                            //    s_ptr[p]->prop3 = *bptr;
                            //    bptr++;
                            //    s_ptr[p]->prop4 = *bptr;
                            //    bptr++;
                            //    s_ptr[p] += s_num;
                            //}
                            //else
                            //    bptr += 4;
                            break;

                        // tag
                        case 12:
                            reader.ReadBytes(4);
                            //if ((x < new_width) && (y < new_height))
                            //{
                            //    p = layout[n] - 12;
                            //    t_ptr[p]->num = (UDWORD) * ((UDWORD*)bptr);
                            //    t_ptr[p] += t_num;
                            //}
                            //bptr += 4;
                            break;
                    }
                }
            }
            ++i;
        }

        if (version >= 2)
        {
            int objectCount = reader.ReadInt32();
            Debug.Log("Objects " + objectCount);

            for (int n = 0; n < objectCount; n++)
            {
                int type = reader.ReadInt32();
                int id = reader.ReadInt32();
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();

                if (version > 5)
                {
                    int flags = reader.ReadInt32();
                }
            }
        }

        stream.Close();

        sw.Stop();
        Debug.Log("DS1 loaded in " + sw.Elapsed);

        var result = new ImportResult();
        result.center = Iso.MapToWorld(new Vector3(width, height)) / 2 / Iso.tileSize;
        result.center.y = -result.center.y;
        return result;
    }

    static GameObject CreateFloor(DT1.Tile tile, int orderInLayer)
    {
        var texture = tile.texture;

        GameObject gameObject = new GameObject();
        gameObject.name = "floor_" + tile.mainIndex + "_" + tile.subIndex;
        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        var meshFilter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = "generated floor mesh";
        mesh.vertices = new Vector3[] { new Vector3(-1, 0.5f), new Vector3(-1, -0.5f), new Vector3(1, -0.5f), new Vector3(1, 0.5f) };
        mesh.triangles = new int[] { 2, 1, 0, 3, 2, 0 };
        float x0 = tile.textureX;
        float y0 = tile.textureY;
        mesh.uv = new Vector2[] {
                  new Vector2 (x0 / texture.width, -y0 / texture.height),
                  new Vector2 (x0 / texture.width, (-y0 -80f) / texture.height),
                  new Vector2 ((x0 + 160f) / texture.width, (-y0 -80f) / texture.height),
                  new Vector2 ((x0 + 160f) / texture.width, -y0 / texture.height)
        };
        meshFilter.mesh = mesh;

        meshRenderer.material = tile.material;
        meshRenderer.sortingLayerName = "Floor";
        meshRenderer.sortingOrder = orderInLayer;
        return gameObject;
    }
}
