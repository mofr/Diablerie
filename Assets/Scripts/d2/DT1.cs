using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DT1
{
    public struct Tile
    {
        public int direction;
        public short roofHeight;
        public byte soundIndex;
        public byte animated;
        public int height;
        public int width;
        public int orientation;
        public int mainIndex;
        public int subIndex;
        public int rarity;
        public byte[] flags;
        public int blockHeaderPointer;
        public int blockDatasLength;
        public int blockCount;

        public Material material;
        public Texture2D texture;
        public Color32[] texturePixels;
        public int textureX;
        public int textureY;
        public int index;

        public void Read(BinaryReader reader)
        {
            direction = reader.ReadInt32();
            roofHeight = reader.ReadInt16();
            soundIndex = reader.ReadByte();
            animated = reader.ReadByte();
            height = reader.ReadInt32();
            width = reader.ReadInt32();
            reader.ReadBytes(4); // zeros
            orientation = reader.ReadInt32();
            mainIndex = reader.ReadInt32();
            subIndex = reader.ReadInt32();
            rarity = reader.ReadInt32();
            reader.ReadBytes(4); // unknown
            flags = reader.ReadBytes(25); // Left to Right, and Bottom to Up
            reader.ReadBytes(7); // unused
            blockHeaderPointer = reader.ReadInt32();
            blockDatasLength = reader.ReadInt32();
            blockCount = reader.ReadInt32();
            reader.ReadBytes(12); // zeros
            index = Index(mainIndex, subIndex, orientation);
        }

        static public int Index(int mainIndex, int subIndex, int orientation)
        {
            return (((mainIndex << 6) + subIndex) << 5) + orientation;
        }
    }

    public struct ImportResult
    {
        public Tile[] tiles;
        public Texture2D[] textures;
    }

    static Dictionary<string, ImportResult> cache = new Dictionary<string, ImportResult>();

    static public void ResetCache()
    {
        cache.Clear();
    }

    static public ImportResult Import(string dt1Path)
    {
        if(cache.ContainsKey(dt1Path))
        {
            return cache[dt1Path];
        }
        
        var importResult = new ImportResult();
        var bytes = File.ReadAllBytes(dt1Path);
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);
        int version1 = reader.ReadInt32();
        int version2 = reader.ReadInt32();
        if (version1 != 7 || version2 != 6)
        {
            Debug.Log(string.Format("Can't read dt1 file, bad version ({0}.{1})", version1, version2));
            return importResult;
        }
        reader.ReadBytes(260);
        int tileCount = reader.ReadInt32();
        reader.ReadInt32(); //  Pointer in file to Tile Headers (= 276) 
        Tile[] tiles = new Tile[tileCount];

        const int textureSize = 2048;
        var textures = new List<Texture2D>();
        var texturesPixels = new List<Color32[]>();
        var packer = new TexturePacker(textureSize, textureSize);
        Material material = null;
        Texture2D texture = null;
        Color32[] pixels = null;

        for (int i = 0; i < tileCount; ++i)
        {
            tiles[i].Read(reader);
            var result = packer.put(tiles[i].width, -tiles[i].height);
            if (result.newTexture)
            {
                texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
                texture.filterMode = FilterMode.Point;
                textures.Add(texture);
                material = new Material(Shader.Find("Sprite"));
                material.name += "(" + dt1Path + ")";
                material.mainTexture = texture;
                pixels = new Color32[textureSize * textureSize];
                texturesPixels.Add(pixels);
            }

            tiles[i].textureX = result.x;
            tiles[i].textureY = result.y;
            tiles[i].texture = texture;
            tiles[i].material = material;
            tiles[i].texturePixels = pixels;

            if ((tiles[i].orientation == 0 || tiles[i].orientation == 15) && tiles[i].height != 0)
            {
                // floor or roof
                tiles[i].height = -79;
            }
            else if (tiles[i].orientation > 0 && tiles[i].orientation < 15)
            {
                tiles[i].textureY += (-tiles[i].height);
            }
        }

        Debug.Log(dt1Path + ", tiles " + tileCount + ", " + textures.Count + " textures");
        for (int i = 0; i < tileCount; ++i)
        {
            var tile = tiles[i];

            if (tile.width == 0 || tile.height == 0)
            {
                Debug.Log(string.Format("Zero size {0}x{1}", tile.width, tile.height));
                continue;
            }

            stream.Seek(tile.blockHeaderPointer, SeekOrigin.Begin);
            for (int block = 0; block < tile.blockCount; ++block)
            {
                int x = reader.ReadInt16();
                int y = reader.ReadInt16();
                reader.ReadBytes(2); // zeros
                reader.ReadByte(); // gridX
                reader.ReadByte(); // gridY
                short format = reader.ReadInt16();
                int length = reader.ReadInt32();
                reader.ReadBytes(2); // zeros
                int fileOffset = reader.ReadInt32();
                int blockDataPosition = tile.blockHeaderPointer + fileOffset;

                if (format == 1)
                {
                    drawBlockIsometric(tile.texturePixels, textureSize, tile.textureX + x, tile.textureY + y, bytes, blockDataPosition, length);
                }
                else
                {
                    drawBlockNormal(tile.texturePixels, textureSize, tile.textureX + x, tile.textureY + y, bytes, blockDataPosition, length);
                }
            }
        }

        for (int i = 0; i < textures.Count; ++i)
        {
            textures[i].SetPixels32(texturesPixels[i]);
            textures[i].Apply();
        }

        importResult.tiles = tiles;
        importResult.textures = textures.ToArray();
        cache[dt1Path] = importResult;
        return importResult;
    }

    static public void ConvertToPng(string assetPath)
    {
        Palette.LoadPalette(1);
        ImportResult result = Import(assetPath);
        int i = 0;
        foreach(var texture in result.textures)
        {
            var pngData = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            var pngPath = assetPath + "." + i + ".png";
            File.WriteAllBytes(pngPath, pngData);
            AssetDatabase.ImportAsset(pngPath);
            ++i;
        }
    }

    static void drawBlockNormal(Color32[] texturePixels, int textureSize, int x0, int y0, byte[] data, int ptr, int length)
    {
        int dst = texturePixels.Length - y0 * textureSize - textureSize + x0;
        int x = 0;
        int y = 0;

        while (length > 0)
        {
            byte b1 = data[ptr];
            byte b2 = data[ptr + 1];
            ptr += 2;
            length -= 2;
            if (b1 != 0 || b2 != 0)
            {
                x += b1;
                length -= b2;
                while (b2 != 0)
                {
                    texturePixels[dst + x] = Palette.palette[data[ptr]];
                    ptr++;
                    x++;
                    b2--;
                }
            }
            else
            {
                x = 0;
                y++;
                dst -= textureSize;
            }
        }
    }

    static int[] xjump = { 14, 12, 10, 8, 6, 4, 2, 0, 2, 4, 6, 8, 10, 12, 14 };
    static int[] nbpix = { 4, 8, 12, 16, 20, 24, 28, 32, 28, 24, 20, 16, 12, 8, 4 };

    static void drawBlockIsometric(Color32[] texturePixels, int textureSize, int x0, int y0, byte[] data, int ptr, int length)
    {
        int dst = texturePixels.Length - y0 * textureSize - textureSize + x0;
        int x, y = 0, n;

        // 3d-isometric subtile is 256 bytes, no more, no less 
        Debug.Assert(length == 256);
        if (length != 256)
            return;

        while (length > 0)
        {
            x = xjump[y];
            n = nbpix[y];
            length -= n;
            while (n != 0)
            {
                texturePixels[dst + x] = Palette.palette[data[ptr]];
                ptr++;
                x++;
                n--;
            }
            y++;
            dst -= textureSize;
        }
    }
}
