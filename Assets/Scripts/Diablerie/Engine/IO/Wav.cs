using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Wav
{
    private const long DataChunkHeader = 0x61746164;
    
    struct Header
    {
        public byte[] riffID; // "riff"
        public uint size;
        public byte[] wavID;  // "WAVE"
        public byte[] fmtID;  // "fmt "
        public uint fmtSize;
        public ushort format;  // PCM = 1, otherwise something compressed 
        public ushort channels;
        public uint sampleRate;
        public uint bytePerSec;
        public ushort blockAlign;
        public ushort bitsPerSample;
    }

    struct DataChunk
    {
        public int position;
        public int sampleOffset;
        public int sampleCount;
        public int blockCount;
    }

    static Header ReadHeader(BinaryReader reader)
    {
        var header = new Header();
        header.riffID = reader.ReadBytes(4);
        header.size = reader.ReadUInt32();
        header.wavID = reader.ReadBytes(4);
        header.fmtID = reader.ReadBytes(4);
        header.fmtSize = reader.ReadUInt32();
        header.format = reader.ReadUInt16();
        header.channels = reader.ReadUInt16();
        header.sampleRate = reader.ReadUInt32();
        header.bytePerSec = reader.ReadUInt32();
        header.blockAlign = reader.ReadUInt16();
        header.bitsPerSample = reader.ReadUInt16();
        return header;
    }

    static float BytesToFloat(byte byte1, byte byte2)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((byte2 << 8) | byte1);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    private static float ReadSample(BinaryReader reader)
    {
        byte byte1 = reader.ReadByte();
        byte byte2 = reader.ReadByte();
        return BytesToFloat(byte1, byte2);
    }

    public static AudioClip Load(string name, bool stream, Stream wavStream)
    {
        var reader = new BinaryReader(wavStream);
        var header = ReadHeader(reader);

        if (header.format != 1)
        {
            Debug.LogWarning("Only PCM wav is implemented (" + name + ")");
            return null;
        }

        if (header.bitsPerSample != 16)
        {
            Debug.LogWarning("Only 16bit wav is implemented (" + name + ")");
            return null;
        }
        
        long startPosition = wavStream.Position;
        var chunks = ScanDataChunks(reader, header);
        int lengthSamples = 0;
        foreach (var chunk in chunks)
        {
            lengthSamples += chunk.blockCount;
        }
        wavStream.Position = startPosition;
        int chunkIndex = 0;
        int sampleIndex = 0;
        int totalRead = 0;
        
        // TODO extract reader as a class with its own state (current chunk index etc), probably use Wav
        var audioClip = AudioClip.Create(name, lengthSamples, header.channels, (int)header.sampleRate, stream, (float[] data) =>
        {
            int i = 0;
            while (i < data.Length && chunkIndex < chunks.Count)
            {
                var chunk = chunks[chunkIndex];
                int samplesAvailable = chunk.sampleCount + chunk.sampleOffset - sampleIndex;
                int samplesToRead = Math.Min(data.Length - i, samplesAvailable);
                for (int j = 0; j < samplesToRead; ++j, ++i)
                {
                    data[i] = ReadSample(reader);
                }
                sampleIndex += samplesToRead;
                totalRead += samplesToRead;
                if (i < data.Length)
                    ++chunkIndex;
            }
        }, (int pos) =>
        {
            for (chunkIndex = 0; chunkIndex < chunks.Count; ++chunkIndex)
            {
                var chunk = chunks[chunkIndex];
                if (pos >= chunk.sampleOffset && pos < chunk.sampleOffset + chunk.sampleCount)
                {
                    wavStream.Position = chunk.position + (pos - chunk.sampleOffset) * header.blockAlign;
                    break;
                }
            }
            sampleIndex = pos;
        });
        
        return audioClip;
    }

    private static List<DataChunk> ScanDataChunks(BinaryReader reader, Header header)
    {
        var chunks = new List<DataChunk>();
        int sampleOffset = 0;
        try
        {
            while (true)
            {
                long dataHeader = reader.ReadUInt32();
                uint dataSize = reader.ReadUInt32();
                if (dataHeader == DataChunkHeader)  // Skipping PAD and other chunks 
                {
                    int blockCount = (int) dataSize / header.blockAlign;
                    int sampleCount = blockCount * header.channels;
                    chunks.Add(new DataChunk()
                    {
                        position = (int) reader.BaseStream.Position,
                        sampleOffset = sampleOffset,
                        sampleCount = sampleCount,
                        blockCount = blockCount,
                    });
                    sampleOffset += sampleCount;
                }
                reader.BaseStream.Seek(dataSize, SeekOrigin.Current);
            }
        }
        catch (IOException)
        {
        }

        return chunks;
    }
}
