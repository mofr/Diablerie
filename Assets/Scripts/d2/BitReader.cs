using System.IO;

public class BitReader
{
    private Stream stream;
    private int current;
    private int index = 8;

    public BitReader(Stream stream)
    {
        this.stream = stream;
    }

    public int ReadBit()
    {
        if (index >= 8)
        {
            current = stream.ReadByte();
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
}
