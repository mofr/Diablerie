public class BitReader
{
    private byte[] data;
    private int bytesRead = 0;
    private int bitsRead = 0;

    public BitReader(byte[] data, long offset = 0)
    {
        this.data = data;
        this.offset = offset;
    }

    public void Skip(int bits)
    {
        var bitsRead = this.bitsRead + bits;
        bytesRead += bitsRead >> 3;
        this.bitsRead = bitsRead & 7;
    }

    public int ReadBool()
    {
        int result = data[bytesRead];
        result &= 1 << bitsRead;
        Skip(1);
        return result;
    }

    public int ReadBitsUnaligned(int bits)
    {
        int result = data[bytesRead];
        result >>= bitsRead;
        result &= (1 << bits) - 1;
        Skip(bits);
        return result;
    }

    public int ReadBits(int count)
    {
        int result = 0;
        int cursor = 0;

        if (bitsRead != 0)
        {
            int remains = 8 - bitsRead;
            int toRead = count > remains ? remains : count;
            result = ReadBitsUnaligned(toRead);
            count -= toRead;
            cursor += toRead;
        }

        while (bitsRead == 0 && count >= 8)
        {
            result |= data[bytesRead] << cursor;
            ++bytesRead;
            count -= 8;
            cursor += 8;
        }

        if (count > 0)
        {
            int tmp = ReadBitsUnaligned(count);
            result |= tmp << cursor;
        }
        return result;
    }

    public int ReadByte()
    {
        if (bitsRead == 0)
        {
            return data[bytesRead++];
        }
        else
        {
            int result = (data[bytesRead++]) | (data[bytesRead] << 8);
            result >>= bitsRead;
            result &= 255;
            return result;
        }
    }

    public int ReadLessThanByte(int count)
    {
        int result;
        int newBitsRead = bitsRead + count;
        if (bitsRead + count > 8)
        {
            result = (data[bytesRead]) | (data[bytesRead + 1] << 8);
        }
        else
        {
            result = data[bytesRead];
        }
        result >>= bitsRead;
        result &= (1 << count) - 1;
        if (newBitsRead > 7)
        {
            bitsRead = newBitsRead & 7;
            ++bytesRead;
        }
        else
        {
            bitsRead = newBitsRead;
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

    public long offset
    {
        get { return bytesRead * 8 + bitsRead; }
        set
        {
            bytesRead = (int)value / 8;
            bitsRead = (int)(value % 8);
        }
    }
}
