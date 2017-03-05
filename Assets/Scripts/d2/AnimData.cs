using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct AnimData
{
    public string cofName;
    public int framesPerDir;
    public int speed;
    public byte[] flags;

    public static bool Find(string name, ref AnimData animData)
    {
        byte hash = Hash(name);
        if (buckets[hash].data == null)
            return false;

        foreach(var data in buckets[hash].data)
        {
            if (data.cofName == name)
            {
                animData = data;
                return true;
            }
        }
        return false;
    }

    struct Bucket
    {
        public AnimData[] data;
    }

    static Bucket[] buckets = new Bucket[256];

    static byte Hash(string name)
    {
        string upperName = name.ToUpper();
        int nb = name.Length;
        byte hash = 0;

        for (int i = 0; i < name.Length; ++i)
        {
            if (name[i] == '.')
                nb = i;
        }
        for (int i = 0; i < nb; i++)
            hash += (byte) upperName[i];
        return hash;
    }

    static AnimData()
    {
        byte[] bytes = File.ReadAllBytes(Application.streamingAssetsPath + "/d2/data/global/animdata.d2");
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);
        while (stream.Position < stream.Length)
        {
            int count = reader.ReadInt32();
            var bucket = new Bucket();
            bucket.data = new AnimData[count];
            byte hash = 0;
            for (int i = 0; i < count; ++i)
            {
                var animData = new AnimData();
                animData.cofName = System.Text.Encoding.Default.GetString(reader.ReadBytes(7));
                reader.ReadByte(); // zero byte from zero-terminated string
                animData.framesPerDir = reader.ReadInt32();
                animData.speed = reader.ReadInt32();
                animData.flags = reader.ReadBytes(144);
                bucket.data[i] = animData;
                if (i == 0)
                    hash = Hash(animData.cofName);
            }
            buckets[hash] = bucket;
        }
    }
}
