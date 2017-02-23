using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class COF
{
    public struct Layer
    {
        public string dccFilename;
        public string name;
        public bool presented;
    }

    public struct ImportResult
    {
        public Layer[] layers;
    }

    static readonly string[] layerNames = { "HD", "TR", "LG", "RA", "LA", "RH", "LH", "SH", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" };
    static Dictionary<string, ImportResult> cache = new Dictionary<string, ImportResult>();

    static public ImportResult Load(Obj obj)
    {
        string _base = obj._base.Replace('\\', '/');
        string token = obj.token;
        string mode = obj.mode;
        string _class = obj._class;
        
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

        result.layers = new Layer[layerCount];

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
            string sptr;
            if (compositIndex == 0)
                sptr = obj.HD;
            else if (compositIndex == 1)
                sptr = obj.TR;
            else if (compositIndex == 2)
                sptr = obj.LG;
            else if (compositIndex == 3)
                sptr = obj.RA;
            else if (compositIndex == 4)
                sptr = obj.LA;
            else if (compositIndex == 5)
                sptr = obj.RH;
            else if (compositIndex == 6)
                sptr = obj.LH;
            else if (compositIndex == 7)
                sptr = obj.SH;
            else if (compositIndex == 8)
                sptr = obj.S1;
            else if (compositIndex == 9)
                sptr = obj.S2;
            else if (compositIndex == 10)
                sptr = obj.S3;
            else if (compositIndex == 11)
                sptr = obj.S4;
            else if (compositIndex == 12)
                sptr = obj.S5;
            else if (compositIndex == 13)
                sptr = obj.S6;
            else if (compositIndex == 14)
                sptr = obj.S7;
            else if (compositIndex == 15)
                sptr = obj.S8;
            else
                continue;
            result.layers[i].dccFilename = "Assets/d2/" + _base + "/" + token + "/" + compositName + "/" + token + compositName + sptr + mode + weaponClass + ".dcc";
            result.layers[i].name = compositName + " " + sptr;
            result.layers[i].presented = true;
        }

        cache.Add(cofFilename, result);
        return result;
    }
}
