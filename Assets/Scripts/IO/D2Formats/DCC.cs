using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DCC : Spritesheet
{
    public List<Texture2D> textures;
    new public int directionCount;
    public int framesPerDirection;

    string filename;
    byte[] bytes;
    Header header;
    Sprite[][] sprites;

    const int DCC_MAX_PB_ENTRY = 85000;

    struct Cell
    {
        public int x0, y0;
        public int w, h;
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

        public Texture2D texture;
        public Color32[] texturePixels;
        public int textureX;
        public int textureY;
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
            if (bitReader.ReadBool() != 0)
            {
                dir.pixel_values[idx] = (byte)i;
                ++idx;
            }
        }

        long offset = bitReader.offset;
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
    }

    static FrameBuffer CreateFrameBuffer(IntRect box)
    {
        FrameBuffer frameBuffer = new FrameBuffer();
        frameBuffer.nb_cell_w = 1 + ((box.width - 1) / 4);
        frameBuffer.nb_cell_h = 1 + ((box.height - 1) / 4);
        frameBuffer.cells = new Cell[frameBuffer.nb_cell_w * frameBuffer.nb_cell_h];
        int[] cell_w = new int[frameBuffer.nb_cell_w];
        int[] cell_h = new int[frameBuffer.nb_cell_h];

        if (frameBuffer.nb_cell_w == 1)
            cell_w[0] = box.width;
        else
        {
            for (int i = 0; i < (frameBuffer.nb_cell_w - 1); i++)
                cell_w[i] = 4;
            cell_w[frameBuffer.nb_cell_w - 1] = box.width - (4 * (frameBuffer.nb_cell_w - 1));
        }

        if (frameBuffer.nb_cell_h == 1)
            cell_h[0] = box.height;
        else
        {
            for (int i = 0; i < (frameBuffer.nb_cell_h - 1); i++)
                cell_h[i] = 4;
            cell_h[frameBuffer.nb_cell_h - 1] = box.height - (4 * (frameBuffer.nb_cell_h - 1));
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
                x0 += cell_w[x];
            }
            y0 += cell_h[y];
        }
    }

    static PixelBufferEntry[] pixelBuffer = new PixelBufferEntry[DCC_MAX_PB_ENTRY];

    static void FillPixelBuffer(Header header, FrameBuffer frameBuffer, Direction dir, Streams streams)
    {
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
            int buffCellIndex = cell0_x + cell0_y * frameBuffer.nb_cell_w;
            int frameCellIndex = 0;
            for (int y = 0; y < frame.nb_cell_h; ++y)
            {
                for (int x = 0; x < frame.nb_cell_w; ++x, ++frameCellIndex)
                {
                    int curr_cell = buffCellIndex + x;
                    if (cellBuffer[curr_cell].val != null)
                    {
                        if (streams.equalCell != null && streams.equalCell.ReadBool() != 0)
                            continue;

                        pixelMask = streams.pixelMask.ReadLessThanByte(4);
                    }
                    else
                        pixelMask = 0x0f;

                    read_pixel[0] = read_pixel[1] = read_pixel[2] = read_pixel[3] = 0;
                    int last_pixel = 0;
                    int nb_pix = nb_pix_table[pixelMask];
                    int encodingType = 0;
                    if (nb_pix != 0 && streams.encodingType != null)
                    {
                        encodingType = streams.encodingType.ReadBool();
                    }

                    int decoded_pix = 0;
                    for (int i = 0; i < nb_pix; i++)
                    {
                        if (encodingType != 0)
                        {
                            read_pixel[i] = streams.rawPixel.ReadByte();
                        }
                        else
                        {
                            read_pixel[i] = last_pixel;
                            int pix_displ = streams.pixelCode.ReadLessThanByte(4);
                            read_pixel[i] += pix_displ;
                            while (pix_displ == 15)
                            {
                                pix_displ = streams.pixelCode.ReadLessThanByte(4);
                                read_pixel[i] += pix_displ;
                            }
                        }

                        if (read_pixel[i] == last_pixel)
                        {
                            read_pixel[i] = 0; // discard this pixel
                            break; // stop the decoding of pixels
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
                    var newEntry = new PixelBufferEntry();
                    newEntry.val = new byte[4];
                    int curr_idx = decoded_pix - 1;

                    for (int i = 0; i < 4; i++)
                    {
                        if ((pixelMask & (1 << i)) != 0)
                        {
                            if (curr_idx >= 0) // if stack is not empty, pop it
                                newEntry.val[i] = (byte)read_pixel[curr_idx--];
                        }
                        else
                            newEntry.val[i] = old_entry.val[i];
                    }
                    newEntry.frame = f;
                    newEntry.frameCellIndex = frameCellIndex;
                    pixelBuffer[pb_idx] = newEntry;
                    cellBuffer[curr_cell] = newEntry;
                }

                buffCellIndex += frameBuffer.nb_cell_w;
            }
        }

        for (int i = 0; i <= pb_idx; i++)
        {
            for (int x = 0; x < 4; x++)
            {
                int y = pixelBuffer[i].val[x];
                pixelBuffer[i].val[x] = dir.pixel_values[y];
            }
        }

        pixelBuffer[pb_idx + 1].frame = -1;
    }

    static void MakeFrames(Header header, Direction dir, FrameBuffer frameBuffer, Streams streams, List<Texture2D> textures, Sprite[] sprites)
    {
        const int padding = 2;
        int textureWidth = Mathf.NextPowerOfTwo((dir.box.width + padding) * header.framesPerDir);
        int textureHeight = Mathf.NextPowerOfTwo(dir.box.height + padding);
        textureWidth = Mathf.Min(1024, textureWidth);

        var packer = new TexturePacker(textureWidth, textureHeight);
        Texture2D texture = null;
        Color32[] pixels = null;

        for (int c = 0; c < frameBuffer.cells.Length; c++)
        {
            frameBuffer.cells[c].w = -1;
            frameBuffer.cells[c].h = -1;
        }

        int pb_idx = 0;
        PixelBufferEntry pbe = pixelBuffer[pb_idx];

        for (int f = 0; f < header.framesPerDir; f++)
        {
            Frame frame = dir.frames[f];

            var pack = packer.put(dir.box.width + padding, dir.box.height + padding);
            if (pack.newTexture)
            {
                if (texture != null)
                {
                    texture.SetPixels32(pixels);
                    texture.Apply(false);
                }
                texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
                pixels = new Color32[textureWidth * textureHeight];
                textures.Add(texture);
            }
            frame.texture = texture;
            frame.texturePixels = pixels;
            frame.textureX = pack.x;
            frame.textureY = pack.y;

            var textureRect = new Rect(frame.textureX, frame.textureY, dir.box.width, dir.box.height);
            var pivot = new Vector2(-dir.box.xMin / (float)dir.box.width, dir.box.yMax / (float)dir.box.height);
            sprites[f] = Sprite.Create(texture, textureRect, pivot, Iso.pixelsPerUnit, extrude: 0, meshType: SpriteMeshType.FullRect);

            int nb_cell = frame.nb_cell_w * frame.nb_cell_h;
            for (int c = 0; c < nb_cell; c++)
            {
                Cell cell = frame.cells[c];

                // buffer cell
                int cell_x = cell.x0 >> 2;
                int cell_y = cell.y0 >> 2;
                int cell_idx = cell_x + (cell_y * frameBuffer.nb_cell_w);

                // equal cell checks
                if ((pbe.frame != f) || (pbe.frameCellIndex != c))
                {
                    // this buffer cell have an equalcell bit set to 1 so copy the frame cell
                    Cell buff_cell = frameBuffer.cells[cell_idx];
                    if (cell.w == buff_cell.w && cell.h == buff_cell.h)
                    {
                        Frame refFrame = dir.frames[f - 1];
                        int textureY = refFrame.textureY + dir.box.height - buff_cell.y0;
                        int textureX = refFrame.textureX + buff_cell.x0;
                        int srcOffset = textureWidth * textureY + textureX;
                        textureY = frame.textureY + dir.box.height - cell.y0;
                        textureX = frame.textureX + cell.x0;
                        int dstOffset = textureWidth * textureY + textureX;
                        for (int y = 0; y < cell.h; y++)
                        {
                            System.Array.Copy(refFrame.texturePixels, srcOffset, frame.texturePixels, dstOffset, cell.w);
                            srcOffset -= textureWidth;
                            dstOffset -= textureWidth;
                        }
                    }
                }
                else
                {
                    // fill the frame cell with pixels

                    if (pbe.val[0] == pbe.val[1])
                    {
                        // fill FRAME cell to color val[0]
                        //clear_to_color(cell->bmp, pbe->val[0]);
                    }
                    else
                    {
                        int nb_bit;
                        if (pbe.val[1] == pbe.val[2])
                            nb_bit = 1;
                        else
                            nb_bit = 2;

                        // fill FRAME cell with pixels
                        int textureY = frame.textureY + dir.box.height - cell.y0;
                        int textureX = frame.textureX + cell.x0;
                        int offset = textureWidth * textureY + textureX;
                        for (int y = 0; y < cell.h; ++y)
                        {
                            for (int x = 0; x < cell.w; ++x)
                            {
                                int pix = streams.pixelCode.ReadLessThanByte(nb_bit);
                                Color32 color = Palette.palette[pbe.val[pix]];
                                frame.texturePixels[offset + x] = color;
                            }
                            offset -= textureWidth;
                        }
                    }

                    // next pixelbuffer entry
                    pb_idx++;
                    pbe = pixelBuffer[pb_idx];
                }

                // for the buffer cell that was used by this frame cell,
                // save the width & size of the current frame cell
                // (needed for further tests about equalcell)
                // and save its origin, for further copy when equalcell
                frameBuffer.cells[cell_idx] = cell;
            }
        }

        if (texture != null)
        {
            texture.SetPixels32(pixels);
            texture.Apply(false);
        }
    }

    readonly static int[] widthTable = { 0, 1, 2, 4, 6, 8, 10, 12, 14, 16, 20, 24, 26, 28, 30, 32 };
    readonly static int[] nb_pix_table = { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };
    readonly static int[] dirs1 = new int[] { 0 };
    readonly static int[] dirs4 = new int[] { 0, 1, 2, 3 };
    readonly static int[] dirs8 = new int[] { 4, 0, 5, 1, 6, 2, 7, 3 };
    readonly static int[] dirs16 = new int[] { 4, 8, 0, 9, 5, 10, 1, 11, 6, 12, 2, 13, 7, 14, 3, 15 };
    readonly static int[] dirs32 = new int[] { 4, 16, 8, 17, 0, 18, 9, 19, 5, 20, 10, 21, 1, 22, 11, 23, 6, 24, 12, 25, 2, 26, 13, 27, 7, 28, 14, 29, 3, 30, 15, 31 };

    public override Sprite[] GetSprites(int d)
    {
        if (sprites[d] == null)
            DecodeDirection(d);

        return sprites[d];
    }

    void DecodeDirection(int d)
    {
        int[] dirs = null;
        switch (header.directionCount)
        {
            case 1: dirs = dirs1; break;
            case 4: dirs = dirs4; break;
            case 8: dirs = dirs8; break;
            case 16: dirs = dirs16; break;
            case 32: dirs = dirs32; break;
            default:
                Debug.LogError("Invalid DCC direction count: " + header.directionCount);
                return;
        }

        UnityEngine.Profiling.Profiler.BeginSample("DCC.DecodeDirection");

        sprites[d] = new Sprite[header.framesPerDir];
        var bitReader = new BitReader(bytes, header.dirOffset[dirs[d]] * 8);
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
        }

        if (optionalBytesSum != 0)
            Debug.LogWarning("optionalBytesSum != 0, not tested");
        bitReader.ReadBits(optionalBytesSum * 8);

        Streams streams = new Streams();
        ReadStreamsInfo(bitReader, dir, bytes, streams);

        FrameBuffer frameBuffer = CreateFrameBuffer(dir.box); // dcc_prepare_buffer_cells
        FillPixelBuffer(header, frameBuffer, dir, streams); // dcc_fill_pixel_buffer
        MakeFrames(header, dir, frameBuffer, streams, textures, sprites[d]); // dcc_make_frames

        UnityEngine.Profiling.Profiler.EndSample();
    }

    static public DCC Load(string filename, bool loadAllDirections = false, bool mpq = true)
    {
        UnityEngine.Profiling.Profiler.BeginSample("DCC.Load");
        var bytes = mpq ? Mpq.ReadAllBytes(filename) : File.ReadAllBytes(filename);
        DCC dcc = Load(filename, bytes, loadAllDirections);
        UnityEngine.Profiling.Profiler.EndSample();
        return dcc;
    }

    static DCC Load(string filename, byte[] bytes, bool loadAllDirections = false)
    {
        DCC dcc = new DCC();
        dcc.bytes = bytes;
        dcc.header = new Header();
        dcc.textures = new List<Texture2D>();
        dcc.filename = filename;

        using (var stream = new MemoryStream(dcc.bytes))
        using (var reader = new BinaryReader(stream))
        {
            ReadHeader(reader, dcc.header);
        }

        dcc.directionCount = dcc.header.directionCount;
        dcc.framesPerDirection = dcc.header.framesPerDir;
        dcc.sprites = new Sprite[dcc.header.directionCount][];

        if (loadAllDirections)
            for (int d = 0; d < dcc.header.directionCount; ++d)
                dcc.DecodeDirection(d);

        return dcc;
    }
}