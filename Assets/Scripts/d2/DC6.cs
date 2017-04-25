using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DC6
{
    public struct Frame
    {
        public int width;
        public int height;
        public Texture2D texture;
        public int textureX;
        public int textureY;
    }

    public int directionCount;
    public int framesPerDirection;
    public Frame[] frames;
    public List<Texture2D> textures = new List<Texture2D>();

    static public DC6 Load(string filename, int textureSize = 512, bool mpq = true, bool loadAllDirections = false)
    {
        Palette.LoadPalette(0);
        var bytes = mpq ? Mpq.ReadAllBytes(filename) : File.ReadAllBytes(filename);

        using (var stream = new MemoryStream(bytes))
        using (var reader = new BinaryReader(stream))
        {
            int dc6_ver1 = reader.ReadInt32();
            var dc6_ver2 = reader.ReadInt32();
            var dc6_ver3 = reader.ReadInt32();
            if ((dc6_ver1 != 6) || (dc6_ver2 != 1) || (dc6_ver3 != 0))
            {
                Debug.LogWarning("Unknown dc6 version " + dc6_ver1 + " " + dc6_ver2 + " " + dc6_ver3);
                return null;
            }

            return Load(stream, reader, textureSize, loadAllDirections);
        }
    }

    static DC6 Load(Stream stream, BinaryReader reader, int textureSize, bool loadAllDirections = false)
    {
        var dc6 = new DC6();
        reader.ReadInt32();
        dc6.directionCount = reader.ReadInt32();
        dc6.framesPerDirection = reader.ReadInt32();
        dc6.frames = new Frame[dc6.framesPerDirection];
        var fptr = stream.Position;

        int textureWidth = textureSize;
        int textureHeight = textureSize;

        Texture2D texture = null;
        var pixels = new Color32[textureWidth * textureHeight];
        var packer = new TexturePacker(textureWidth, textureHeight, padding: 2);
        byte[] data = new byte[1024];

        int dir = 0;
        for (int i = 0; i < dc6.framesPerDirection; i++)
        {
            stream.Seek(fptr + (dir * dc6.framesPerDirection + i) * 4, SeekOrigin.Begin);
            long offset = reader.ReadInt32();
            stream.Seek(offset, SeekOrigin.Begin);

            var frame = new Frame();

            reader.ReadInt32();
            frame.width = reader.ReadInt32();
            frame.height = reader.ReadInt32();
            reader.ReadInt32(); // f_offx
            reader.ReadInt32(); // f_offy
            reader.ReadInt32();
            reader.ReadInt32();
            int f_len = reader.ReadInt32();
            if (data.Length < f_len)
                data = new byte[f_len];
            reader.Read(data, 0, f_len);

            var pack = packer.put(frame.width, frame.height);
            if (pack.newTexture)
            {
                if (texture != null)
                {
                    texture.SetPixels32(pixels);
                    texture.Apply();
                }
                texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
                pixels = new Color32[textureWidth * textureHeight];
                dc6.textures.Add(texture);
            }
            drawFrame(data, f_len, pixels, textureSize, pack.x, pack.y + frame.height);

            frame.texture = texture;
            frame.textureX = pack.x;
            frame.textureY = pack.y;
            dc6.frames[i] = frame;
        }

        if (texture != null)
        {
            texture.SetPixels32(pixels);
            texture.Apply();
        }

        return dc6;
    }

    static void drawFrame(byte[] data, int size, Color32[] pixels, int textureSize, int x0, int y0)
    {
        int dst = textureSize * textureSize - y0 * textureSize - textureSize;
        int ptr = 0;
        int i2, x = x0, y = y0, c, c2;

        for (int i = 0; i < size; i++)
        {
            c = data[ptr];
            ++ptr;

            if (c == 0x80)
            {
                x = x0;
                y--;
                dst += textureSize;
            }
            else if ((c & 0x80) != 0)
                x += c & 0x7F;
            else
            {
                for (i2 = 0; i2 < c; i2++)
                {
                    c2 = data[ptr];
                    ++ptr;
                    i++;
                    pixels[dst + x] = Palette.palette[c2];
                    x++;
                }
            }
        }
    }
}
