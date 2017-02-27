using System.Collections.Generic;
using System.IO;

public class COF
{
    public Layer[] layers;
    public int framesPerDirection;
    public int directionCount;
    public int layerCount;
    public int mode;
    public byte[] priority;

    public struct Layer
    {
        public string dccFilename;
        public string name;
    }

    public static readonly string[][] ModeNames = {
        new string[] { "DT", "NU", "WL", "RN", "GH", "TN", "TW", "A1", "A2", "BL", "SC", "TH", "KK", "S1", "S2", "S3", "S4", "DD", "GH", "GH" }, // player (plrmode.txt)
        new string[] { "DT", "NU", "WL", "GH", "A1", "A2", "BL", "SC", "S1", "S2", "S3", "S4", "DD", "GH", "xx", "RN" }, // monsters (monmode.txt)
        new string[] { "NU", "OP", "ON", "S1", "S2", "S3", "S4", "S5" } // objects (objmode.txt)
    };
    static public readonly string[] layerNames = { "HD", "TR", "LG", "RA", "LA", "RH", "LH", "SH", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" };
    static Dictionary<string, COF> cache = new Dictionary<string, COF>();

    static public COF Load(Obj obj)
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

        COF cof = new COF();

        byte[] bytes = File.ReadAllBytes(cofFilename);
        var stream = new MemoryStream(bytes);
        var reader = new BinaryReader(stream);

        cof.layerCount = reader.ReadByte();
        cof.framesPerDirection = reader.ReadByte();
        cof.directionCount = reader.ReadByte();
        cof.mode = System.Array.IndexOf(ModeNames[obj.type], mode);
        stream.Seek(25, SeekOrigin.Current);

        cof.layers = new Layer[16];

        for (int i = 0; i < cof.layerCount; ++i)
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
            string sptr = obj.layers[compositIndex];
            cof.layers[compositIndex].dccFilename = "Assets/d2/" + _base + "/" + token + "/" + compositName + "/" + token + compositName + sptr + mode + weaponClass + ".dcc";
            cof.layers[compositIndex].name = compositName + " " + sptr;
        }

        stream.Seek(cof.framesPerDirection, SeekOrigin.Current);
        cof.priority = reader.ReadBytes(cof.directionCount * cof.framesPerDirection * cof.layerCount);

        AnimData animData = new AnimData();
        if (AnimData.Find(token + mode + _class, ref animData))
        {
            //Debug.Log(cofFilename + " " + framesPerDirection + " anim data found " + animData.framesPerDir + " " + animData.speed);
        }

        cache.Add(cofFilename, cof);
        return cof;
    }
}
