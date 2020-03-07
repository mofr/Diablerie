using System.Collections.Generic;
using System.IO;
using Diablerie.Engine.LibraryExtensions;
using UnityEngine;

namespace Diablerie.Engine.IO.D2Formats
{
    public class COF
    {
        public Layer[] layers;
        public Layer[] compositLayers;
        public int framesPerDirection;
        public int directionCount;
        public int layerCount;
        public byte[] priority;
        public float frameDuration = 1.0f / 25.0f;
        public string basePath;
        public string token;
        public string mode;

        public struct Layer
        {
            public int index;
            public int compositIndex;
            public string name;
            public string weaponClass;
            public bool shadow;
            public Material material;
        }

        // plrmode.txt
        public static readonly string[] PlayerModes = {"DT", "NU", "WL", "RN", "GH", "TN", "TW", "A1", "A2", "BL", "SC", "TH", "KK", "S1", "S2", "S3", "S4", "DD", "GH", "GH"};
        
        // monmode.txt
        public static readonly string[] MonsterModes = {"DT", "NU", "WL", "GH", "A1", "A2", "BL", "SC", "S1", "S2", "S3", "S4", "DD", "GH", "xx", "RN"};
        
        // objmode.txt
        public static readonly string[] StaticObjectModes = {"NU", "OP", "ON", "S1", "S2", "S3", "S4", "S5"}; 
        
        public static readonly string[] layerNames = { "HD", "TR", "LG", "RA", "LA", "RH", "LH", "SH", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8" };
        static Dictionary<string, COF> cache = new Dictionary<string, COF>();

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

        public static COF Load(string basePath, string token, string weaponClass, string mode)
        {
            var filename = basePath + @"\" + token + @"\cof\" + token + mode + weaponClass + ".cof";
            if (cache.ContainsKey(filename))
            {
                return cache[filename];
            }

            UnityEngine.Profiling.Profiler.BeginSample("COF.Load");

            COF cof = new COF();
            cof.basePath = basePath;
            cof.token = token;
            cof.mode = mode;

            byte[] bytes = Mpq.ReadAllBytes(filename);
            using (var stream = new MemoryStream(bytes))
            using (var reader = new BinaryReader(stream))
            {
                cof.layerCount = reader.ReadByte();
                cof.framesPerDirection = reader.ReadByte();
                cof.directionCount = reader.ReadByte();
                stream.Seek(25, SeekOrigin.Current);

                cof.compositLayers = new Layer[16];
                cof.layers = new Layer[cof.layerCount];

                for (int i = 0; i < cof.layerCount; ++i)
                {
                    Layer layer = new Layer();
                    layer.index = i;
                    layer.compositIndex = reader.ReadByte();
                    layer.name = layerNames[layer.compositIndex];

                    layer.shadow = reader.ReadByte() != 0;
                    reader.ReadByte();

                    bool transparent = reader.ReadByte() != 0;
                    int blendMode = reader.ReadByte();
                    if (transparent)
                    {
                        layer.material = Materials.SoftAdditive;
                    }
                    else
                    {
                        layer.material = Materials.Normal;
                    }

                    layer.weaponClass = System.Text.Encoding.Default.GetString(reader.ReadBytes(4), 0, 3);

                    cof.layers[i] = layer;
                    cof.compositLayers[layer.compositIndex] = layer;
                }

                stream.Seek(cof.framesPerDirection, SeekOrigin.Current);
                cof.priority = reader.ReadBytes(cof.directionCount * cof.framesPerDirection * cof.layerCount);
            }

            AnimData animData = new AnimData();
            if (AnimData.Find(token + mode + weaponClass, ref animData))
            {
                cof.frameDuration = animData.frameDuration;
                float refFrameCount = referenceFrameCount.GetValueOrDefault(token + mode, animData.framesPerDir);
                cof.frameDuration *= animData.framesPerDir / refFrameCount;
            }
            else
            {
                Debug.LogWarning("animdata not found " + (token + mode + weaponClass));
            }

            cache.Add(filename, cof);

            UnityEngine.Profiling.Profiler.EndSample();
            return cof;
        }

        public string GetSpritesheetFilename(Layer layer, string equip)
        {
            return basePath + @"\" + token + @"\" + layer.name + @"\" + token + layer.name + equip + mode + layer.weaponClass;
        }
    }
}
