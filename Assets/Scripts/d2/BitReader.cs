public class BitReader
{
    private byte[] bytes;
    private int byteIndex = 0;
    private int currentByte;
    public int bitIndex = 8;

    public BitReader(byte[] bytes, long offset = 0)
    {
        this.bytes = bytes;
        this.offset = offset;
    }

    public int ReadBit()
    {
        if (bitIndex >= 8)
        {
            currentByte = bytes[byteIndex++];
            bitIndex %= 8;
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

    readonly static int[] mask = { 0, 1, 3, 7, 15, 31, 63, 127, 255 };
    readonly static int[] revMask = { 255, 254, 252, 248, 240, 224, 192, 128 };

    public int ReadByte()
    {
        return ReadBits(8);
        //int result = currentByte & revMask[bitIndex];
        //if (bitIndex > 0)
        //{
        //    currentByte = bytes[byteIndex++];
        //    result += currentByte & mask[bitIndex];
        //}
        //return result;

        if (bitIndex >= 8)
        {
            currentByte = bytes[byteIndex++];
            bitIndex %= 8;
        }

        int result = currentByte >> bitIndex;
        if (bitIndex > 0)
        {
            currentByte = bytes[byteIndex++];
            result += (currentByte & mask[bitIndex]) << bitIndex;
        }
        bitIndex += 8;
        return result;
    }

    readonly static int[] revMask4 = { 15, 30, 60, 120, 240, 224, 192, 128 };

    public int ReadBits4()
    {
        if (bitIndex >= 8)
        {
            currentByte = bytes[byteIndex++];
            bitIndex %= 8;
        }

        int result = (currentByte & revMask4[bitIndex]) >> bitIndex;
        if (bitIndex > 4)
        {
            currentByte = bytes[byteIndex++];
            bitIndex -= 4;
            result += (currentByte & mask[bitIndex]) << (4 - bitIndex);
        }
        else
            bitIndex += 4;
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
        set
        {
            byteIndex = (int)value / 8;
            bitIndex = (int)(value % 8);
            currentByte = bytes[byteIndex++];
        }
    }
}
