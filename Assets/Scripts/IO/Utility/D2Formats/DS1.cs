using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DS1
{
    public string filename;
    public int width;
    public int height;
    int version;
    public Cell[][] walls;
    public Cell[][] floors;
    public ObjectSpawnInfo[] objects;
    public List<Group> groups;
    public string[] dt1Files;
    public DT1.Sampler tileSampler;

    public struct ObjectSpawnInfo
    {
        public int x;
        public int y;
        public Obj obj;
    }

    public struct Group
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    public struct Cell
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

    static Dictionary<string, DS1> cache = new Dictionary<string, DS1>();

    static public void ResetCache()
    {
        cache.Clear();
    }

    static public DS1 Load(string filename, bool mpq = true)
    {
        UnityEngine.Profiling.Profiler.BeginSample("DS1.Load");

        string lowerFilename = filename.ToLower();
        if (cache.ContainsKey(lowerFilename))
        {
            UnityEngine.Profiling.Profiler.EndSample();
            return cache[lowerFilename];
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();
        byte[] bytes = mpq ? Mpq.ReadAllBytes(filename) : File.ReadAllBytes(filename);
        var ds1 = Load(bytes);
        ds1.filename = filename;
        Debug.Log(Path.GetFileName(filename) + " loaded in " + sw.ElapsedMilliseconds + " ms");
        cache[lowerFilename] = ds1;
        UnityEngine.Profiling.Profiler.EndSample();
        return ds1;
    }

    static DS1 Load(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        using (var reader = new BinaryReader(stream))
        {
            DS1 ds1 = new DS1();
            ds1.version = reader.ReadInt32();
            ds1.width = reader.ReadInt32() + 1;
            ds1.height = reader.ReadInt32() + 1;

            int act = 0;
            if (ds1.version >= 8)
            {
                act = reader.ReadInt32();
                act = Mathf.Min(act, 4);
            }

            int tagType = 0;
            if (ds1.version >= 10)
            {
                tagType = reader.ReadInt32();
            }

            if (ds1.version >= 3)
            {
                Palette.LoadPalette(act);
                ds1.dt1Files = ReadDependencies(reader);
                ds1.tileSampler = new DT1.Sampler();
                foreach (var dt1Filename in ds1.dt1Files)
                {
                    var dt1 = DT1.Load(dt1Filename);
                    ds1.tileSampler.Add(dt1.tiles);
                }
            }

            if ((ds1.version >= 9) && (ds1.version <= 13))
                stream.Seek(8, SeekOrigin.Current);

            ReadLayers(ds1, bytes, reader, stream, tagType);
            ReadObjects(ds1, reader, act);
            try
            {
                ReadGroups(ds1, reader, tagType);
            }
            catch (EndOfStreamException)
            {
                // in fact there can be less groups than expected
            }

            return ds1;
        }
    }

    private static void ReadGroups(DS1 ds1, BinaryReader reader, int tagType)
    {
        bool hasGroups = ds1.version >= 12 && (tagType == 1 || tagType == 2);
        if (!hasGroups)
            return;

        if (ds1.version >= 18)
            reader.ReadInt32();
        int groupCount = reader.ReadInt32();
        ds1.groups = new List<Group>();

        for (int i = 0; i < groupCount; i++)
        {
            var group = new Group();
            group.x = reader.ReadInt32();
            group.y = reader.ReadInt32();
            group.width = reader.ReadInt32();
            group.height = reader.ReadInt32();
            if (ds1.version >= 13)
            {
                reader.ReadInt32(); // unknown
            }
            ds1.groups.Add(group);
        }
    }

    private static void ReadObjects(DS1 ds1, BinaryReader reader, int act)
    {
        if (ds1.version < 2)
            return;
        int objectCount = reader.ReadInt32();
        //Debug.Log("Objects " + objectCount);
        ds1.objects = new ObjectSpawnInfo[objectCount];

        for (int i = 0; i < objectCount; i++)
        {
            var info = new ObjectSpawnInfo();
            int type = reader.ReadInt32();
            int id = reader.ReadInt32();
            info.x = reader.ReadInt32();
            info.y = reader.ReadInt32();

            if (ds1.version > 5)
            {
                reader.ReadInt32(); // flags
            }

            info.obj = Obj.Find(act, type, id);
            ds1.objects[i] = info;
        }
    }

    static void ReadLayers(DS1 ds1, byte[] bytes, BinaryReader reader, Stream stream, int tagType)
    {
        int wallLayerCount = 1;
        int floorLayerCount = 1;
        int shadowLayerCount = 1;
        int tagLayerCount = 0;

        if (ds1.version >= 4)
        {
            wallLayerCount = reader.ReadInt32();

            if (ds1.version >= 16)
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

        //Debug.Log("layers : (2 * " + wallLayerCount + " walls) + " + floorLayerCount + " floors + " + shadowLayerCount + " shadow + " + tagLayerCount + " tag");

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

        if (ds1.version < 4)
        {
            ReadCells(ds1.walls[0], bytes, stream);
            ReadCells(ds1.floors[0], bytes, stream);
            ReadOrientations(ds1.walls[0], bytes, stream);
            stream.Seek(4 * ds1.width * ds1.height, SeekOrigin.Current); // tag
            stream.Seek(4 * ds1.width * ds1.height, SeekOrigin.Current); // shadow
        }
        else
        {
            for (int i = 0; i < wallLayerCount; i++)
            {
                ReadCells(ds1.walls[i], bytes, stream);
                ReadOrientations(ds1.walls[i], bytes, stream);
            }
            for (int i = 0; i < floorLayerCount; i++)
                ReadCells(ds1.floors[i], bytes, stream);
            if (shadowLayerCount != 0)
                stream.Seek(4 * ds1.width * ds1.height, SeekOrigin.Current); // shadow
            if (tagLayerCount != 0)
                stream.Seek(4 * ds1.width * ds1.height, SeekOrigin.Current); // tag
        }

        for (int w = 0; w < wallLayerCount; w++)
        {
            var cells = ds1.walls[w];
            int i = 0;
            for (int y = 0; y < ds1.height; y++)
            {
                for (int x = 0; x < ds1.width; x++, i++)
                {
                    var cell = cells[i];
                    if (cell.prop1 == 0)
                        continue;

                    if (ds1.version < 7)
                        cell.orientation = dirLookup[cell.orientation];

                    cell.mainIndex = (cell.prop3 >> 4) + ((cell.prop4 & 0x03) << 4);
                    cell.subIndex = cell.prop2;
                    cell.tileIndex = DT1.Tile.Index(cell.mainIndex, cell.subIndex, cell.orientation);

                    cells[i] = cell;
                }
            }
        }

        for (int f = 0; f < floorLayerCount; f++)
        {
            var cells = ds1.floors[f];
            for (int i = 0; i < cells.Length; i++)
            {
                var cell = cells[i];

                if (cell.prop1 == 0)
                    continue;

                cell.mainIndex = (cell.prop3 >> 4) + ((cell.prop4 & 0x03) << 4);
                cell.subIndex = cell.prop2;
                cell.orientation = 0;
                cell.tileIndex = DT1.Tile.Index(cell.mainIndex, cell.subIndex, cell.orientation);

                cells[i] = cell;
            }
        }
    }

    static unsafe void ReadCells(Cell[] cells, byte[] bytes, Stream stream)
    {
        long position = stream.Position;

        fixed (Cell* fixedCells = cells)
        fixed (byte* fixedBytes = bytes)
        {
            byte* src = fixedBytes + position;
            Cell* cell = fixedCells;
            for (int i = 0; i < cells.Length; ++i)
            {
                cell->prop1 = *(src++);
                cell->prop2 = *(src++);
                cell->prop3 = *(src++);
                cell->prop4 = *(src++);
                ++cell;
            }
        }

        stream.Seek(4 * cells.Length, SeekOrigin.Current);
    }

    static unsafe void ReadOrientations(Cell[] cells, byte[] bytes, Stream stream)
    {
        long position = stream.Position;
        fixed (Cell* fixedCells = cells)
        fixed (byte* fixedBytes = bytes)
        {
            byte* src = fixedBytes + position;
            Cell* cell = fixedCells;
            for (int i = 0; i < cells.Length; ++i)
            {
                cell->orientation = *src;
                src += 4;
                ++cell;
            }
        }

        stream.Seek(4 * cells.Length, SeekOrigin.Current);
    }

    static string[] ReadDependencies(BinaryReader reader)
    {
        int fileCount = reader.ReadInt32();
        var filenames = new string[fileCount];

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
            filenames[i] = dependency;
        }

        return filenames;
    }
}
