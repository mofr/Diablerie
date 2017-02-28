public class BitReader
{
    private byte[] bytes;
    private int byteIndex = 0;
    private int currentByte;
    public int bitIndex = 8;

    public BitReader(byte[] bytes, long offset = 0)
    {
        this.bytes = bytes;
        byteIndex = (int) offset / 8;
        bitIndex = (int) (offset % 8);
        currentByte = bytes[byteIndex++];
    }

    public int ReadBit()
    {
        if (bitIndex >= 8)
        {
            currentByte = bytes[byteIndex++];
            bitIndex = 0;
        }
        int result = (currentByte >> bitIndex) & 1;
        ++bitIndex;
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
        bitIndex = 8;
    }

    public int bitsLeft
    {
        get { return 8 - bitIndex; }
    }

    public long offset
    {
        get { return byteIndex * 8 - bitsLeft; }
    }
}
