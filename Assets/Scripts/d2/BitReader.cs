using System.IO;

public class BitReader
{
    private Stream _stream;
    private int current;
    private int index = 8;

    public BitReader(Stream stream)
    {
        _stream = stream;
    }

    public BitReader(byte[] bytes, long offset = 0)
    {
        _stream = new MemoryStream(bytes);
        _stream.Seek(offset / 8, SeekOrigin.Begin);
        index = (int) (offset % 8);
    }

    public int ReadBit()
    {
        if (index >= 8)
        {
            current = _stream.ReadByte();
            index = 0;
        }
        int result = (current >> index) & 1;
        ++index;
        return result;
    }

    public int ReadBits(int count)
    {
        int result = 0;
        for (int i = 0; i < count; ++i)
        {
            result += ReadBit() << i;
        }
        return result;
    }

    public int ReadSigned(int count)
    {
        int result = ReadBits(count);
        if ((result & (1 << (count - 1))) != 0)
        {
            // negative : extend its sign
            result |= ~((1 << count) - 1);
        }
        return result;
    }

    public void Reset()
    {
        index = 8;
    }

    public int bitsLeft
    {
        get { return 8 - index; }
    }

    public Stream stream
    {
        get { return _stream; }
    }
}
