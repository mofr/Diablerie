using System.Collections.Generic;
using System.IO;
using Diablerie.Engine.LibraryExtensions;
using UnityEngine;

namespace Diablerie.Engine.IO.D2Formats
{
    public struct AnimData
    {
        public string cofName;
        public int framesPerDir;
        public int speed;
        public byte[] flags;
        public float frameDuration;
        
        // values from charstats.txt
        static Dictionary<string, float> referenceFrameCount = new Dictionary<string, float>() {
            {"AMWL", 6},
            {"AMRN", 4},
            {"SOWL", 8},
            {"SORN", 5},
            {"NEWL", 9},
            {"NERN", 5},
            {"PAWL", 8},
            {"PARN", 5},
            {"BAWL", 7},
            {"BARN", 4},
            {"DZWL", 9},
            {"DZRN", 5},
            {"AIWL", 6},
            {"AIRN", 4},
        };

        public static float GetFrameDuration(string token, string mode, string weaponClass)
        {
            AnimData animData = new AnimData();
            if (!AnimData.Find(token + mode + weaponClass, ref animData))
            {
                Debug.LogWarning("animdata not found " + (token + mode + weaponClass));
                return 1 / 25.0f;
            }

            float refFrameCount = referenceFrameCount.GetValueOrDefault(token + mode, animData.framesPerDir);
            float frameDuration = animData.frameDuration * animData.framesPerDir / refFrameCount;
            return frameDuration;
        }

        public static bool Find(string name, ref AnimData animData)
        {
            name = name.ToUpper();
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
            int hash = 0;
            for (int i = 0; i < nb; i++)
                hash += upperName[i];
            return (byte)(hash & 0xff);
        }

        public static void Load(string filename)
        {
            using (var stream = Mpq.fs.OpenFile(filename))
            using (var reader = new BinaryReader(stream))
            {
                while (stream.Position < stream.Length)
                {
                    int count = reader.ReadInt32();
                    var bucket = new Bucket();
                    bucket.data = new AnimData[count];
                    for (int i = 0; i < count; ++i)
                    {
                        var animData = new AnimData();
                        animData.cofName = System.Text.Encoding.Default.GetString(reader.ReadBytes(8), 0, 7);
                        animData.framesPerDir = reader.ReadInt32();
                        animData.speed = reader.ReadInt32();
                        animData.flags = reader.ReadBytes(144);
                        animData.frameDuration = 256.0f / 25.0f / animData.speed;
                        bucket.data[i] = animData;
                    }
                    if (count > 0)
                    {
                        byte hash = Hash(bucket.data[0].cofName);
                        buckets[hash] = bucket;
                    }
                }
            }
        }
    }
}
