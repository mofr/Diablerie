using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Wav
{
    private const long DataChunkHeader = 0x61746164;

    private Stream _stream;
    private BinaryReader _reader;
    private Header _header;
    private List<DataChunk> _chunks;
    private int _chunkIndex;
    private int _sampleIndex;
    private AudioClip _clip;

    public AudioClip Clip => _clip;

    public static AudioClip Load(string clipName, bool streamData, Stream stream)
    {
        var wav = new Wav(clipName, streamData, stream);
        return wav.Clip;
    }

    public Wav(string clipName, bool streamData, Stream stream)
    {
        _stream = stream;
        _reader = new BinaryReader(stream);
        _header = ReadHeader(_reader);

        if (_header.format != 1)
        {
            Debug.LogWarning("Only PCM wav is implemented (" + clipName + ")");
            return;
        }

        if (_header.bitsPerSample != 16)
        {
            Debug.LogWarning("Only 16bit wav is implemented (" + clipName + ")");
            return;
        }
        
        _chunks = ScanDataChunks(_reader, _header);
        int lengthSamples = 0;
        foreach (var chunk in _chunks)
        {
            lengthSamples += chunk.blockCount;
        }

        _clip = AudioClip.Create(clipName, lengthSamples, _header.channels, (int)_header.sampleRate, streamData, ReadCallback, SetPositionCallback);
    }

    private void ReadCallback(float[] data)
    {
        int i = 0;
        while (i < data.Length && _chunkIndex < _chunks.Count)
        {
            var chunk = _chunks[_chunkIndex];
            int samplesAvailable = chunk.sampleCount + chunk.sampleOffset - _sampleIndex;
            int samplesToRead = Math.Min(data.Length - i, samplesAvailable);
            for (int j = 0; j < samplesToRead; ++j, ++i)
            {
                data[i] = ReadSample(_reader);
            }
            _sampleIndex += samplesToRead;
            if (i < data.Length)
                ++_chunkIndex;
        }
    }

    private void SetPositionCallback(int pos)
    {
        for (_chunkIndex = 0; _chunkIndex < _chunks.Count; ++_chunkIndex)
        {
            var chunk = _chunks[_chunkIndex];
            if (pos >= chunk.sampleOffset && pos < chunk.sampleOffset + chunk.sampleCount)
            {
                _stream.Position = chunk.position + (pos - chunk.sampleOffset) * _header.blockAlign;
                break;
            }
        }
        _sampleIndex = pos;
    }
    
    struct Header
    {
        public uint riffHeader; // "riff"
        public uint size;
        public uint wavHeader;  // "WAVE"
        public uint formatSectionHeader;  // "fmt "
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

    private static Header ReadHeader(BinaryReader reader)
    {
        var header = new Header();
        header.riffHeader = reader.ReadUInt32();
        header.size = reader.ReadUInt32();
        header.wavHeader = reader.ReadUInt32();
        header.formatSectionHeader = reader.ReadUInt32();
        header.fmtSize = reader.ReadUInt32();
        header.format = reader.ReadUInt16();
        header.channels = reader.ReadUInt16();
        header.sampleRate = reader.ReadUInt32();
        header.bytePerSec = reader.ReadUInt32();
        header.blockAlign = reader.ReadUInt16();
        header.bitsPerSample = reader.ReadUInt16();
        return header;
    }

    private static float ReadSample(BinaryReader reader)
    {
        byte byte1 = reader.ReadByte();
        byte byte2 = reader.ReadByte();
        return BytesToFloat(byte1, byte2);
    }

    private static float BytesToFloat(byte byte1, byte byte2)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((byte2 << 8) | byte1);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    private static List<DataChunk> ScanDataChunks(BinaryReader reader, Header header)
    {
        long startPosition = reader.BaseStream.Position;
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
        
        reader.BaseStream.Position = startPosition;

        return chunks;
    }
}
