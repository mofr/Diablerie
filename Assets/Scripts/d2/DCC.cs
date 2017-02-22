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

    const int DCC_MAX_PB_ENTRY = 85000;

    struct Cell
    {
        public int x0, y0;  // for frame cells in stage 2
        public int w, h;

        public int last_w, last_h;   // width & size of the last frame cell that used
                                     // this buffer cell (for stage 2)
        public int last_x0, last_y0;
    }

    class Header
    {
        public byte fileSignature;
        public byte version;
        public byte directionCount;
        public int framesPerDir;
        public int tag;
        public int finalDc6Size;
        public int[] dirOffset;
    }

    class Direction
    {
        public int outsizeCoded;
        public int compressionFlag;
        public int variable0Bits;
        public int widthBits;
        public int heightBits;
        public int xoffsetBits;
        public int yoffsetBits;
        public int optionalBytesBits;
        public int codedBytesBits;

        public IntRect box = IntRect.zero;
        public Frame[] frames;
        public byte[] pixel_values = new byte[256];
    }

    struct FrameBuffer
    {
        public Cell[] cells;
        public int nb_cell_w;
        public int nb_cell_h;
    }

    class Frame
    {
        public int variable0;
        public int width;
        public int height;
        public int xoffset;
        public int yoffset;
        public int optionalBytes;
        public int codedBytes;
        public int bottomUp;
        public IntRect box;

        public Cell[] cells;
        public int nb_cell_w = 0;
        public int nb_cell_h = 0;
    }

    struct PixelBufferEntry
    {
        public byte[] val;
        public int frame;
        public int frameCellIndex;
    }

    class Streams
    {
        public BitReader equalCell;
        public BitReader pixelMask;
        public BitReader encodingType;
        public BitReader rawPixel;
        public BitReader pixelCode;
    }

    static void ReadHeader(BinaryReader reader, Header header)
    {
        header.fileSignature = reader.ReadByte();
        header.version = reader.ReadByte();
        header.directionCount = reader.ReadByte();
        header.framesPerDir = reader.ReadInt32();
        header.tag = reader.ReadInt32();
        header.finalDc6Size = reader.ReadInt32();
        header.dirOffset = new int[header.directionCount];
        for (int dir = 0; dir < header.directionCount; ++dir)
        {
            header.dirOffset[dir] = reader.ReadInt32();
        }
    }

    static void ReadDirection(BitReader bitReader, Direction dir)
    {
        dir.outsizeCoded = bitReader.ReadBits(32);
        dir.compressionFlag = bitReader.ReadBits(2);
        dir.variable0Bits = bitReader.ReadBits(4);
        dir.widthBits = bitReader.ReadBits(4);
        dir.heightBits = bitReader.ReadBits(4);
        dir.xoffsetBits = bitReader.ReadBits(4);
        dir.yoffsetBits = bitReader.ReadBits(4);
        dir.optionalBytesBits = bitReader.ReadBits(4);
        dir.codedBytesBits = bitReader.ReadBits(4);
    }

    static void ReadFrame(BitReader bitReader, Direction dir, Frame frame)
    {
        frame.variable0 = bitReader.ReadBits(widthTable[dir.variable0Bits]);
        frame.width = bitReader.ReadBits(widthTable[dir.widthBits]);
        frame.height = bitReader.ReadBits(widthTable[dir.heightBits]);
        frame.xoffset = bitReader.ReadSigned(widthTable[dir.xoffsetBits]);
        frame.yoffset = bitReader.ReadSigned(widthTable[dir.yoffsetBits]);
        frame.optionalBytes = bitReader.ReadBits(widthTable[dir.optionalBytesBits]);
        frame.codedBytes = bitReader.ReadBits(widthTable[dir.codedBytesBits]);
        frame.bottomUp = bitReader.ReadBits(1);
        frame.box = new IntRect(frame.xoffset, frame.yoffset, frame.width, frame.height);
    }

    static void ReadStreamsInfo(BitReader bitReader, Direction dir, byte[] dcc, Streams streams)
    {
        int equalCellSize = 0;
        int pixelMaskSize = 0;
        int encodingTypeSize = 0;
        int rawPixelSize = 0;

        if ((dir.compressionFlag & 0x02) != 0)
        {
            equalCellSize = bitReader.ReadBits(20);
        }
        pixelMaskSize = bitReader.ReadBits(20);
        if ((dir.compressionFlag & 0x01) != 0)
        {
            encodingTypeSize = bitReader.ReadBits(20);
            rawPixelSize = bitReader.ReadBits(20);
        }

        for (int i = 0, idx = 0; i < 256; ++i)
        {
            if (bitReader.ReadBit() != 0)
            {
                dir.pixel_values[idx] = (byte)i;
                ++idx;
            }
        }

        long offset = bitReader.stream.Position * 8 - bitReader.bitsLeft;
        if (equalCellSize != 0)
            streams.equalCell = new BitReader(dcc, offset);
        offset += equalCellSize;
        if (pixelMaskSize != 0)
            streams.pixelMask = new BitReader(dcc, offset);
        offset += pixelMaskSize;
        if (encodingTypeSize != 0)
            streams.encodingType = new BitReader(dcc, offset);
        offset += encodingTypeSize;
        if (rawPixelSize != 0)
            streams.rawPixel = new BitReader(dcc, offset);
        offset += rawPixelSize;
        streams.pixelCode = new BitReader(dcc, offset);

        Debug.Log("equalCellSize " + equalCellSize);
        Debug.Log("pixelMaskSize " + pixelMaskSize);
        Debug.Log("encodingTypeSize " + encodingTypeSize);
        Debug.Log("rawPixelSize " + rawPixelSize);
        Debug.Log("pcd size " + (dcc.Length * 8 - offset));
    }

    static FrameBuffer CreateFrameBuffer(Direction dir)
    {
        FrameBuffer frameBuffer = new FrameBuffer();
        frameBuffer.nb_cell_w = 1 + ((dir.box.width - 1) / 4);
        frameBuffer.nb_cell_h = 1 + ((dir.box.height - 1) / 4);
        frameBuffer.cells = new Cell[frameBuffer.nb_cell_w * frameBuffer.nb_cell_h];
        int[] cell_w = new int[frameBuffer.nb_cell_w];
        int[] cell_h = new int[frameBuffer.nb_cell_h];

        if (frameBuffer.nb_cell_w == 1)
            cell_w[0] = dir.box.width;
        else
        {
            for (int i = 0; i < (frameBuffer.nb_cell_w - 1); i++)
                cell_w[i] = 4;
            cell_w[frameBuffer.nb_cell_w - 1] = dir.box.width - (4 * (frameBuffer.nb_cell_w - 1));
        }

        if (frameBuffer.nb_cell_h == 1)
            cell_h[0] = dir.box.height;
        else
        {
            for (int i = 0; i < (frameBuffer.nb_cell_h - 1); i++)
                cell_h[i] = 4;
            cell_h[frameBuffer.nb_cell_h - 1] = dir.box.height - (4 * (frameBuffer.nb_cell_h - 1));
        }

        int y0 = 0;
        for (int y = 0; y < frameBuffer.nb_cell_h; y++)
        {
            int x0 = 0;
            for (int x = 0; x < frameBuffer.nb_cell_w; x++)
            {
                int index = x + (y * frameBuffer.nb_cell_w);
                frameBuffer.cells[index].w = cell_w[x];
                frameBuffer.cells[index].h = cell_h[y];
                x0 += 4;
            }
            y0 += 4;
        }

        return frameBuffer;
    }

    static void CreateFrameCells(IntRect box, Frame frame)
    {
        // width (in # of pixels) in 1st column
        int w = 4 - ((frame.box.xMin - box.xMin) % 4);

        frame.nb_cell_w = 0;
        frame.nb_cell_h = 0;

        if ((frame.width - w) <= 1) // if 2nd column is 0 or 1 pixel width
            frame.nb_cell_w = 1;
        else
        {
            // so, we have minimum 2 pixels behind 1st column
            int tmp = frame.width - w - 1; // tmp is minimum 1, can't be 0
            frame.nb_cell_w = 2 + (tmp / 4);
            if ((tmp % 4) == 0)
                frame.nb_cell_w--;
        }

        int h = 4 - ((frame.box.yMin - box.yMin) % 4);
        if ((frame.height - h) <= 1)
            frame.nb_cell_h = 1;
        else
        {
            int tmp = frame.height - h - 1;
            frame.nb_cell_h = 2 + (tmp / 4);
            if ((tmp % 4) == 0)
                frame.nb_cell_h--;
        }

        frame.cells = new Cell[frame.nb_cell_w * frame.nb_cell_h];
        int[] cell_w = new int[frame.nb_cell_w];
        int[] cell_h = new int[frame.nb_cell_h];
        
        if (frame.nb_cell_w == 1)
            cell_w[0] = frame.width;
        else
        {
            cell_w[0] = w;
            for (int i = 1; i < (frame.nb_cell_w - 1); i++)
                cell_w[i] = 4;
            cell_w[frame.nb_cell_w - 1] = frame.width - w - (4 * (frame.nb_cell_w - 2));
        }

        if (frame.nb_cell_h == 1)
            cell_h[0] = frame.height;
        else
        {
            cell_h[0] = h;
            for (int i = 1; i < (frame.nb_cell_h - 1); i++)
                cell_h[i] = 4;
            cell_h[frame.nb_cell_h - 1] = frame.height - h - (4 * (frame.nb_cell_h - 2));
        }

        int y0 = frame.box.yMin - box.yMin;
        for (int y = 0; y < frame.nb_cell_h; y++)
        {
            int x0 = frame.box.xMin - box.xMin;
            for (int x = 0; x < frame.nb_cell_w; x++)
            {
                int index = x + (y * frame.nb_cell_w);
                frame.cells[index].x0 = x0;
                frame.cells[index].y0 = y0;
                frame.cells[index].w = cell_w[x];
                frame.cells[index].h = cell_h[y];
                //cell->bmp = create_sub_bitmap(dir->bmp, x0, y0, cell->w, cell->h);
                x0 += cell_w[x];
            }
            y0 += cell_h[y];
        }
    }

    static void FillPixelBuffer(Header header, Direction dir, Streams streams)
    {
        FrameBuffer frameBuffer = CreateFrameBuffer(dir); // dcc_prepare_buffer_cells
        Debug.Log("cells " + frameBuffer.nb_cell_w + "x" + frameBuffer.nb_cell_h);

        PixelBufferEntry[] pixelBuffer = new PixelBufferEntry[DCC_MAX_PB_ENTRY];
        for(int i = 0; i < pixelBuffer.Length; ++i)
        {
            pixelBuffer[i].val = new byte[4];
        }
        PixelBufferEntry[] cellBuffer = new PixelBufferEntry[frameBuffer.cells.Length];
        int pixelMask = 0;
        int[] read_pixel = new int[4];
        int pb_idx = -1;

        for (int f = 0; f < header.framesPerDir; ++f)
        {
            Frame frame = dir.frames[f];
            int cell0_x = (frame.box.xMin - dir.box.xMin) / 4;
            int cell0_y = (frame.box.yMin - dir.box.yMin) / 4;
            CreateFrameCells(dir.box, frame); // dcc_prepare_frame_cells
            for (int y = 0; y < frame.nb_cell_h; y++)
            {
                int curr_cell_y = cell0_y + y;
                for (int x = 0; x < frame.nb_cell_w; x++)
                {
                    int curr_cell_x = cell0_x + x;
                    int curr_cell = curr_cell_x + (curr_cell_y * frameBuffer.nb_cell_w);
                    bool nextCell = false;
                    if (cellBuffer[curr_cell].val != null)
                    {
                        int tmp = 0;
                        if (streams.equalCell != null)
                            tmp = streams.equalCell.ReadBit();

                        if (tmp == 0)
                            pixelMask = streams.pixelMask.ReadBits(4);
                        else
                            nextCell = true;
                    }
                    else
                        pixelMask = 0x0f;

                    if (!nextCell)
                    {
                        read_pixel[0] = read_pixel[1] = read_pixel[2] = read_pixel[3] = 0;
                        int last_pixel = 0;
                        int nb_pix = nb_pix_table[pixelMask];
                        int encodingType = 0;
                        if (nb_pix != 0 && streams.encodingType != null)
                        {
                            encodingType = streams.encodingType.ReadBit();
                        }

                        int decoded_pix = 0;
                        for (int i = 0; i < nb_pix; i++)
                        {
                            if (encodingType != 0)
                            {
                                read_pixel[i] = streams.rawPixel.ReadBits(8);
                            }
                            else
                            {
                                read_pixel[i] = last_pixel;
                                int pix_displ = streams.pixelCode.ReadBits(4);
                                read_pixel[i] += pix_displ;
                                while (pix_displ == 15)
                                {
                                    pix_displ = streams.pixelCode.ReadBits(4);
                                    read_pixel[i] += pix_displ;
                                }
                            }

                            if (read_pixel[i] == last_pixel)
                            {
                                read_pixel[i] = 0; // discard this pixel
                                i = nb_pix;        // stop the decoding of pixels
                            }
                            else
                            {
                                last_pixel = read_pixel[i];
                                decoded_pix++;
                            }
                        }

                        // we have the 4 pixels code for the new entry in pixel_buffer
                        PixelBufferEntry old_entry = cellBuffer[curr_cell];
                        pb_idx++;
                        Debug.Assert(pb_idx < DCC_MAX_PB_ENTRY);
                        var newEntry = pixelBuffer[pb_idx];
                        int curr_idx = decoded_pix - 1;

                        for (int i = 0; i < 4; i++)
                        {
                            if ((pixelMask & (1 << i)) != 0)
                            {
                                if (curr_idx >= 0) // if stack is not empty, pop it
                                    newEntry.val[i] = (byte)read_pixel[curr_idx--];
                                else // else pop a 0
                                    newEntry.val[i] = 0;
                            }
                            else
                                newEntry.val[i] = old_entry.val[i];
                        }
                        newEntry.frame = f;
                        newEntry.frameCellIndex = x + (y * frame.nb_cell_w);
                        pixelBuffer[pb_idx] = newEntry;
                        cellBuffer[curr_cell] = newEntry;
                    }
                }
            }
        }
    }

    static Dictionary<string, ImportResult> cache = new Dictionary<string, ImportResult>();
    static int[] widthTable = { 0, 1, 2, 4, 6, 8, 10, 12, 14, 16, 20, 24, 26, 28, 30, 32 };
    static int[] nb_pix_table = {0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4};

    static public ImportResult Load(string filename, bool ignoreCache = false)
    {
        Debug.Log("Loading " + filename);
        filename = filename.ToLower();
        if (!ignoreCache && cache.ContainsKey(filename))
        {
            return cache[filename];
        }

        ImportResult result = new ImportResult();
        result.textures = new List<Texture2D>();
        var sprites = new List<Sprite>();

        const int textureSize = 512;
        var packer = new TexturePacker(textureSize, textureSize);
        Texture2D texture = null;
        Color32[] pixels = null;

        byte[] dcc = File.ReadAllBytes(filename);
        var stream = new MemoryStream(dcc);
        var reader = new BinaryReader(stream);
        var bitReader = new BitReader(stream);

        Header header = new Header();
        ReadHeader(reader, header);

        for (int d = 0; d < header.directionCount; ++d)
        {
            Debug.Log("direction " + d);
            stream.Seek(header.dirOffset[d], SeekOrigin.Begin);
            bitReader.Reset();
            Direction dir = new Direction();
            ReadDirection(bitReader, dir);

            int optionalBytesSum = 0;
            dir.frames = new Frame[header.framesPerDir];

            for (int f = 0; f < header.framesPerDir; ++f)
            {
                Frame frame = new Frame();
                dir.frames[f] = frame;
                ReadFrame(bitReader, dir, frame);

                optionalBytesSum += frame.optionalBytes;

                if (frame.bottomUp != 0)
                {
                    Debug.LogWarning("BottomUp frames are not implemented yet (" + filename + ")");
                    continue;
                }

                if (f == 0)
                    dir.box = frame.box;
                else
                {
                    dir.box.xMin = Mathf.Min(dir.box.xMin, frame.box.xMin);
                    dir.box.yMin = Mathf.Min(dir.box.yMin, frame.box.yMin);
                    dir.box.xMax = Mathf.Max(dir.box.xMax, frame.box.xMax);
                    dir.box.yMax = Mathf.Max(dir.box.yMax, frame.box.yMax);
                }

                int padding = 2;
                var pack = packer.put(frame.width + padding, frame.height + padding);
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
                int debugCornerWidth = Mathf.Min(10, frame.width);
                int debugCornerHeight = Mathf.Min(10, frame.height);
                for (int i = 0; i < debugCornerWidth; ++i)
                    pixels[textureSize * (pack.y + frame.height) + pack.x + i] = Color.red;
                for (int i = 0; i < debugCornerWidth; ++i)
                    pixels[textureSize * (pack.y + frame.height - 1) + pack.x + i] = Color.red;
                for (int i = 0; i < debugCornerHeight; ++i)
                    pixels[textureSize * (pack.y + frame.height - i) + pack.x] = Color.red;
                for (int i = 0; i < debugCornerHeight; ++i)
                    pixels[textureSize * (pack.y + frame.height - i) + pack.x + 1] = Color.red;
                for (int i = 0; i < debugCornerWidth; ++i)
                    pixels[textureSize * pack.y + pack.x - i + frame.width] = Color.blue;
                for (int i = 0; i < debugCornerHeight; ++i)
                    pixels[textureSize * (pack.y + i) + pack.x + frame.width] = Color.blue;
                for (int i = 0; i < debugCornerWidth; ++i)
                    pixels[textureSize * (pack.y + 1) + pack.x - i + frame.width] = Color.blue;
                for (int i = 0; i < debugCornerHeight; ++i)
                    pixels[textureSize * (pack.y + i) + pack.x + frame.width - 1] = Color.blue;

                var spriteRect = new Rect(pack.x, pack.y, frame.width, frame.height);
                var pivot = new Vector2(-frame.xoffset / (float)frame.width, frame.yoffset / (float)frame.height);
                Sprite sprite = Sprite.Create(texture, spriteRect, pivot, Iso.pixelsPerUnit);
                sprites.Add(sprite);
            }

            if (optionalBytesSum != 0)
                Debug.LogWarning("optionalBytesSum != 0, not tested");
            stream.Seek(optionalBytesSum, SeekOrigin.Current);

            Streams streams = new Streams();
            ReadStreamsInfo(bitReader, dir, dcc, streams);

            Debug.Log("box " + dir.box.AsString());

            FillPixelBuffer(header, dir, streams); // dcc_fill_pixel_buffer
        }

        if (texture != null)
        {
            texture.SetPixels32(pixels);
            texture.Apply();
        }

        result.anim = ScriptableObject.CreateInstance<IsoAnimation>();
        result.anim.directionCount = header.directionCount;
        result.anim.states = new IsoAnimation.State[1];
        result.anim.states[0] = new IsoAnimation.State();
        result.anim.states[0].name = "Generated from DCC";
        result.anim.states[0].sprites = sprites.ToArray();

        if (!ignoreCache)
            cache.Add(filename, result);
        return result;
    }

    static public void ConvertToPng(string assetPath)
    {
        Palette.LoadPalette(1);
        ImportResult result = Load(assetPath, ignoreCache: true);
        int i = 0;
        foreach (var texture in result.textures)
        {
            var pngData = texture.EncodeToPNG();
            Object.DestroyImmediate(texture);
            var pngPath = assetPath + "." + i + ".png";
            File.WriteAllBytes(pngPath, pngData);
            AssetDatabase.ImportAsset(pngPath);
            ++i;
        }
    }
}