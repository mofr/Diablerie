using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class COF
{
    public Layer[] layers;
    public Layer[] compositLayers;
    public int framesPerDirection;
    public int directionCount;
    public int layerCount;
    public byte[] priority;
    public float frameDuration = 1.0f / 12.0f;

    public struct Layer
    {
        public int index;
        public string dccFilename;
        public string name;
        public Material material;
    }

    public static readonly string[][] ModeNames = {
        new string[] { "DT", "NU", "WL", "RN", "GH", "TN", "TW", "A1", "A2", "BL", "SC", "TH", "KK", "S1", "S2", "S3", "S4", "DD", "GH", "GH" }, // player (plrmode.txt)
        new string[] { "DT", "NU", "WL", "GH", "A1", "A2", "BL", "SC", "S1", "S2", "S3", "S4", "DD", "GH", "xx", "RN" }, // monsters (monmode.txt)
        new string[] { "NU", "OP", "ON", "S1", "S2", "S3", "S4", "S5" } // objects (objmode.txt)
    };
    static public readonly string[] layerNames = { "HD", "TR", "LG", "RA", "LA", "RH", "LH", "SH", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" };
    static Dictionary<string, COF> cache = new Dictionary<string, COF>();

    static public COF Load(string basePath, string token, string _class, string[] gear, string mode)
    {
        string cofFilename = Application.streamingAssetsPath + "/d2/" + basePath + "/" + token + "/cof/" + token + mode + _class + ".cof";
        cofFilename.ToLower();
        if (cache.ContainsKey(cofFilename))
        {
            return cache[cofFilename];
        }

        COF cof = new COF();

        byte[] bytes = File.ReadAllBytes(cofFilename);
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);

        cof.layerCount = reader.ReadByte();
        cof.framesPerDirection = reader.ReadByte();
        cof.directionCount = reader.ReadByte();
        stream.Seek(25, SeekOrigin.Current);

        cof.compositLayers = new Layer[16];
        cof.layers = new Layer[cof.layerCount];

        for (int i = 0; i < cof.layerCount; ++i)
        {
            int compositIndex = reader.ReadByte();
            string compositName = layerNames[compositIndex];

            // shadows
            reader.ReadByte();
            reader.ReadByte();

            bool transparent = reader.ReadByte() != 0;
            int blendMode = reader.ReadByte();

            string weaponClass = System.Text.Encoding.Default.GetString(reader.ReadBytes(3));
            reader.ReadByte(); // zero byte from zero-terminated weapon class string
            string sptr = gear[compositIndex];
            if (sptr == null || sptr == "")
                continue;
            cof.compositLayers[compositIndex].dccFilename = (Application.streamingAssetsPath + "/d2/" + basePath + "/" + token + "/" + compositName + "/" + token + compositName + sptr + mode + weaponClass + ".dcc").ToLower();
            cof.compositLayers[compositIndex].name = compositName;
            cof.compositLayers[compositIndex].index = i;

            if (transparent)
            {
                cof.compositLayers[compositIndex].material = Materials.softAdditive;
            }
            else
            {
                cof.compositLayers[compositIndex].material = Materials.normal;
            }

            cof.layers[i] = cof.compositLayers[compositIndex];
        }

        stream.Seek(cof.framesPerDirection, SeekOrigin.Current);
        cof.priority = reader.ReadBytes(cof.directionCount * cof.framesPerDirection * cof.layerCount);

        AnimData animData = new AnimData();
        if (AnimData.Find(token + mode + _class, ref animData))
        {
            cof.frameDuration = animData.frameDuration;
        }

        cache.Add(cofFilename, cof);
        return cof;
    }
}
