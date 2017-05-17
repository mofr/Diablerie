using UnityEngine;
using System.IO;

public class Wav
{
    struct Header
    {
        public byte[] riffID; // "riff"
        public uint size;
        public byte[] wavID;  // "WAVE"
        public byte[] fmtID;  // "fmt "
        public uint fmtSize;
        public ushort format;
        public ushort channels;
        public uint sampleRate;
        public uint bytePerSec;
        public ushort blockAlign;
        public ushort bitsPerSample;
        public byte[] dataID;
        public uint dataSize;
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
        header.dataID = reader.ReadBytes(4);
        header.dataSize = reader.ReadUInt32();
        return header;
    }

    static float BytesToFloat(byte byte1, byte byte2)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((byte2 << 8) | byte1);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    public static AudioClip Load(string name, byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        using (var reader = new BinaryReader(stream))
        {
            var header = ReadHeader(reader);

            if (header.bitsPerSample != 16)
            {
                Debug.LogWarning("Only 16bit wav loading is implemented (" + name + ")");
                return null;
            }

            int sampleCount = (int)header.dataSize / header.bitsPerSample * 8;
            var audioClip = AudioClip.Create(name, sampleCount, header.channels, (int)header.sampleRate, false);
            float[] data = new float[sampleCount];
            for (int i = 0; i < data.Length; ++i)
            {
                byte byte1 = reader.ReadByte();
                byte byte2 = reader.ReadByte();
                data[i] = BytesToFloat(byte1, byte2);
            }

            audioClip.SetData(data, 0);
            return audioClip;
        }
    }
}
