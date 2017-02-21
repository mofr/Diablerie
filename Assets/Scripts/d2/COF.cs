using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class COF
{
    public struct ImportResult
    {
        public string[] layers;
    }

    static readonly string[] layerNames = { "HD", "TR", "LG", "RA", "LA", "RH", "LH", "SH", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" };
    static Dictionary<string, ImportResult> cache = new Dictionary<string, ImportResult>();

    static public ImportResult Load(string _base, string token, string mode, string _class)
    {
        _base = _base.Replace('\\', '/');
        string cofFilename = "Assets/d2/" + _base + "/" + token + "/cof/" + token + mode + _class + ".cof";
        cofFilename.ToLower();
        if (cache.ContainsKey(cofFilename))
        {
            return cache[cofFilename];
        }

        ImportResult result = new ImportResult();

        byte[] bytes = File.ReadAllBytes(cofFilename);
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);

        byte layerCount = reader.ReadByte();
        byte framesPerDirection = reader.ReadByte();
        byte directionCount = reader.ReadByte();
        stream.Seek(25, SeekOrigin.Current);

        result.layers = new string[layerCount];

        for (int i = 0; i < layerCount; ++i)
        {
            int compositIndex = reader.ReadByte();
            string compositName = layerNames[compositIndex];

            // shadows
            reader.ReadByte();
            reader.ReadByte();

            // transparency
            reader.ReadByte();
            reader.ReadByte();

            string weaponClass = System.Text.Encoding.Default.GetString(reader.ReadBytes(3));
            reader.ReadByte(); // zero byte from zero-terminated weapon class string
            string sptr = "lit";
            string filename = "Assets/d2/" + _base + "/" + token + "/" + compositName + "/" + token + compositName + sptr + mode + weaponClass + ".dcc";
            result.layers[i] = filename;
        }

        cache.Add(cofFilename, result);
        return result;
    }
}
