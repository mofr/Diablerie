using System.Collections.Generic;
using NUnit.Framework;

class BitReaderTest
{
    [Test]
    public void ReadByteTest()
    {
        byte[] data = { 105, 210, 210, 210, 45, 150, 75, 165, 210, 210, 210, 210, 210 };

        const int trial_count = 10;

        int[] result = { 105, 52, 154, 77, 38, 147, 73, 164, 210, 105 };

        for (int i = 0; i < trial_count; ++i)
        {
            var reader = new BitReader(data, i);
            Assert.AreEqual(result[i], reader.ReadByte());
        }

        {
            var reader = new BitReader(data);
            for (int i = 0; i < trial_count; ++i)
            {
                reader.offset = i;
                Assert.AreEqual(result[i], reader.ReadByte());
            }
        }

        {
            var reader = new BitReader(data);
            for (int i = 0; i < trial_count; ++i)
            {
                Assert.AreEqual(data[i], reader.ReadByte());
            }
        }
    }

    [Test]
    public void ReadBits4Test()
    {
        byte[] data = { 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210, 210 };
        var reader = new BitReader(data);

        const int trial_count = 10;

        int[] values = new int[trial_count] { 2, 9, 4, 10, 13, 6, 11, 5, 2, 9 };

        for (int i = 0; i < trial_count; ++i)
        {
            reader.offset = i;
            Assert.AreEqual(i, reader.offset);
            Assert.AreEqual(values[i], reader.ReadBits(4));
            Assert.AreEqual(i + 4, reader.offset);
        }

        for (int i = 0; i < trial_count; ++i)
        {
            reader.offset = i;
            Assert.AreEqual(i, reader.offset);
            Assert.AreEqual(values[i], reader.ReadBits4());
            Assert.AreEqual(i + 4, reader.offset);
        }
    }
}
