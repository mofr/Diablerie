using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class DCC
{
    public struct ImportResult
    {
        public List<Texture2D> textures;
        public IsoAnimation anim;
    }

    static public ImportResult Load(string filename)
    {
        ImportResult result = new ImportResult();
        result.textures = new List<Texture2D>();
        var sprites = new List<Sprite>();

        const int textureSize = 2048;
        var packer = new TexturePacker(textureSize, textureSize);
        Texture2D texture = null;
        Color32[] pixels = null;

        byte[] dcc = File.ReadAllBytes(filename);
        var stream = new MemoryStream(dcc);
        var reader = new BinaryReader(stream);
        var bitReader = new BitReader(stream);

        byte fileSignature = reader.ReadByte();
        byte version = reader.ReadByte();
        byte directionCount = reader.ReadByte();
        int framesPerDir = reader.ReadInt32();
        int tag = reader.ReadInt32();
        int finalDc6Size = reader.ReadInt32();
        int[] dirOffset = new int[directionCount];

        for(int dir = 0; dir < directionCount; ++dir)
        {
            dirOffset[dir] = reader.ReadInt32();
        }

        int[] widthTable = { 0, 1, 2, 4, 6, 8, 10, 12, 14, 16, 20, 24, 26, 28, 30, 32 };

        for (int dir = 0; dir < directionCount; ++dir)
        {
            stream.Seek(dirOffset[dir], SeekOrigin.Begin);
            int outsizeCoded = reader.ReadInt32();
            int compressionFlag = bitReader.ReadBits(2);
            int variable0Bits = bitReader.ReadBits(4);
            int widthBits = bitReader.ReadBits(4);
            int heightBits = bitReader.ReadBits(4);
            int xoffsetBits = bitReader.ReadBits(4);
            int yoffsetBits = bitReader.ReadBits(4);
            int optionalBytesBits = bitReader.ReadBits(4);
            int codedBytesBits = bitReader.ReadBits(4);

            int optionalBytesSum = 0;

            for (int f = 0; f < framesPerDir; ++f)
            {
                int variable0 = bitReader.ReadBits(widthTable[variable0Bits]);
                int width = bitReader.ReadBits(widthTable[widthBits]);
                int height = bitReader.ReadBits(widthTable[heightBits]);
                int xoffset = bitReader.ReadSigned(widthTable[xoffsetBits]);
                int yoffset = bitReader.ReadSigned(widthTable[yoffsetBits]);
                int optionalBytes = bitReader.ReadBits(widthTable[optionalBytesBits]);
                int codedBytes = bitReader.ReadBits(widthTable[codedBytesBits]);
                int bottomUp = bitReader.ReadBits(1);

                optionalBytesSum += optionalBytes;

                int padding = 2;
                var pack = packer.put(width + padding, height + padding);
                if (pack.newTexture)
                {
                    if (texture != null)
                    {
                        texture.SetPixels32(pixels);
                        texture.Apply();
                    }
                    texture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
                    pixels = new Color32[textureSize * textureSize];
                    result.textures.Add(texture);
                }

                // debug frame
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * (pack.y + height) + pack.x + i] = Color.red;
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * (pack.y + height - 1) + pack.x + i] = Color.red;
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * (pack.y + height - i) + pack.x] = Color.red;
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * (pack.y + height - i) + pack.x + 1] = Color.red;
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * pack.y + pack.x - i + width] = Color.blue;
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * (pack.y + i) + pack.x + width] = Color.blue;
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * (pack.y + 1) + pack.x - i + width] = Color.blue;
                for (int i = 0; i < 10; ++i)
                    pixels[textureSize * (pack.y + i) + pack.x + width - 1] = Color.blue;

                var spriteRect = new Rect(pack.x, pack.y, width, height);
                var pivot = new Vector2(-xoffset / (float)width, yoffset / (float)height);
                Sprite sprite = Sprite.Create(texture, spriteRect, pivot, Iso.pixelsPerUnit);
                sprites.Add(sprite);
            }

            stream.Seek(optionalBytesSum, SeekOrigin.Current);
            bitReader.Reset();
            if ((compressionFlag & 0x02) != 0)
            {
                int equalCellSize = bitReader.ReadBits(20);
            }

            int pixelMaskSize = bitReader.ReadBits(20);
            if ((compressionFlag & 0x01) != 0)
            {
                int encodingTypeSize = bitReader.ReadBits(20);
                int rawPixelSize = bitReader.ReadBits(20);
            }

            // 256 bits pixels value key
            bitReader.ReadBits(256);
        }

        if (texture != null)
        {
            texture.SetPixels32(pixels);
            texture.Apply();
        }

        result.anim = ScriptableObject.CreateInstance<IsoAnimation>();
        result.anim.directionCount = directionCount;
        result.anim.states = new IsoAnimation.State[1];
        result.anim.states[0] = new IsoAnimation.State();
        result.anim.states[0].name = "Generated from DCC";
        result.anim.states[0].sprites = sprites.ToArray();

        return result;
    }

    static public void ConvertToPng(string assetPath)
    {
        Palette.LoadPalette(1);
        ImportResult result = Load(assetPath);
        int i = 0;
        foreach (var texture in result.textures)
        {
            var pngData = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            var pngPath = assetPath + "." + i + ".png";
            File.WriteAllBytes(pngPath, pngData);
            AssetDatabase.ImportAsset(pngPath);
        }
    }
}