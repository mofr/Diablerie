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
            index = (mainIndex << 6) + subIndex;
        }
    }

    static Color32[] transparentColors = new Color32[2048 * 2048];

    static public Tile[] Import(string dt1Path)
    {
        var stream = new BufferedStream(File.OpenRead(dt1Path));
        var reader = new BinaryReader(stream);
        int version1 = reader.ReadInt32();
        int version2 = reader.ReadInt32();
        if (version1 != 7 || version2 != 6)
        {
            Debug.Log(string.Format("Can't read dt1 file, bad version ({0}.{1})", version1, version2));
            return null;
        }
        reader.ReadBytes(260);
        int tileCount = reader.ReadInt32();
        int tileCountFit = 0;
        reader.ReadInt32(); //  Pointer in file to Tile Headers (= 276) 
        Tile[] tiles = new Tile[tileCount];
        int height = 2048;
        int width = 2048;
        int xPos = 0;
        int yPos = 0;
        int rowHeight = 0;
        for (int i = 0; i < tileCount; ++i)
        {
            tiles[i].Read(reader);
            if (tiles[i].orientation != 0) 
            {
                // only floors (debug)
                i -= 1;
                tileCount -= 1;
                continue;
            }
            tiles[i].textureX = xPos;
            tiles[i].textureY = yPos;
            rowHeight = Mathf.Max(-tiles[i].height, rowHeight);
            if (xPos + tiles[i].width > width)
            {
                xPos = 0;
                yPos += rowHeight;
                rowHeight = -tiles[i].height;
                tiles[i].textureX = 0;
                tiles[i].textureY = yPos;
            }

            xPos += tiles[i].width;

            if (yPos + rowHeight > height)
            {
                break;
            }

            tileCountFit += 1;
        }

        var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels32(transparentColors);

        var material = new Material(Shader.Find("Sprite"));
        material.name += "(" + dt1Path + ")";
        material.mainTexture = texture;

        Debug.Log("tiles fit " + tileCountFit + " / " + tileCount);
        byte[] blockData = new byte[1024];
        for (int i = 0; i < tileCountFit; ++i)
        {
            tiles[i].material = material;
            tiles[i].texture = texture;
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

                var positionBeforeSeek = stream.Position;
                stream.Seek(blockDataPosition, SeekOrigin.Begin);

                if (blockData.Length < length)
                    blockData = new byte[length];
                reader.Read(blockData, 0, length);
                if (tile.orientation > 0 && tile.orientation < 15)
                {
                    y += (-tile.height);
                }
                if (format == 1)
                {
                    drawBlockIsometric(texture, tile.textureX + x, tile.textureY + y, blockData, length);
                }
                else
                {
                    drawBlockNormal(texture, tile.textureX + x, tile.textureY + y, blockData, length);
                }

                stream.Seek(positionBeforeSeek, SeekOrigin.Begin);
            }
        }

        texture.Apply();
        stream.Close();

        return tiles;
    }

    static void drawBlockNormal(Texture2D texture, int x0, int y0, byte[] data, int length)
    {
        int ptr = 0;
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
                    texture.SetPixel(x0 + x, -y0 - y, Palette.palette[data[ptr]]);
                    ptr++;
                    x++;
                    b2--;
                }
            }
            else
            {
                x = 0;
                y++;
            }
        }
    }

    static int[] xjump = { 14, 12, 10, 8, 6, 4, 2, 0, 2, 4, 6, 8, 10, 12, 14 };
    static int[] nbpix = { 4, 8, 12, 16, 20, 24, 28, 32, 28, 24, 20, 16, 12, 8, 4 };

    static void drawBlockIsometric(Texture2D texture, int x0, int y0, byte[] data, int length)
    {
        int ptr = 0;
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
                texture.SetPixel(x0 + x, -y0 - y, Palette.palette[data[ptr]]);
                ptr++;
                x++;
                n--;
            }
            y++;
        }
    }
}
