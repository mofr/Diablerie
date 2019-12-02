using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Diablerie.Engine.LibraryExtensions;
using Diablerie.Engine.Utility;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Diablerie.Engine.IO.D2Formats
{
    public struct Datasheet
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
        public class Sequence : Attribute
        {
            public int length;
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
        public class Record : Attribute
        {
        }

        public interface Loader<T>
        {
            void LoadRecord(ref T record, DatasheetStream stream);
        }

        private static Dictionary<Type, object> loaders = new Dictionary<Type, object>();

        private static void RegisterLoader(Type recordType, object loader)
        {
            loaders.Add(recordType, loader);
        }
    
        private static Loader<T> GetLoader<T>()
        {
            return (Loader<T>) loaders.GetValueOrDefault(typeof(T), null);
        }

        static Datasheet()
        {
            DiscoverLoaders();
        }

        public static List<T> Load<T>(string filename, int headerLines = 1) where T : new()
        {
            Profiler.BeginSample("Datasheet.Load");
            var stopwatch = Stopwatch.StartNew();

            Profiler.BeginSample("File.ReadAllText");
            var fileSw = Stopwatch.StartNew();
            string csv = File.ReadAllText(Application.streamingAssetsPath + "/" + filename);
            var fileReadMs = fileSw.ElapsedMilliseconds;
            Profiler.EndSample();

            Loader<T> loader = GetLoader<T>();
            if (loader == null)
                throw new Exception("Datasheet loader for " + typeof(T) + " not found");

            var splitter = new DatasheetLineSplitter(csv);
            splitter.Skip(headerLines);
            var records = new List<T>(1024);
            int lineIndex = headerLines;
            StringSpan line = new StringSpan();
            var stream = new DatasheetStream();
            while(splitter.ReadLine(ref line))
            {
                if (line.length == 0)
                    continue;

                stream.SetSource(line);
                T obj = new T();
                try
                {
                    loader.LoadRecord(ref obj, stream);
                }
                catch (Exception e)
                {
                    throw new Exception("Datasheet parsing error at line " + lineIndex + ": " + line.str.Substring(0, 32), e);
                }
                records.Add(obj);
            }
            Debug.Log("Load " + filename + " (" + records.Count + " records, elapsed " + stopwatch.Elapsed.Milliseconds + " ms, file read " + fileReadMs + " ms)");
            Profiler.EndSample();
            return records;
        }

        private static void DiscoverLoaders()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsClass)
                    {
                        var interfaces = type.GetInterfaces();
                        if (interfaces.Length != 1)
                            continue;
                        Type @interface = interfaces[0];
                        if (!@interface.IsGenericType)
                            continue;
                        if (@interface.GetGenericTypeDefinition() != typeof(Loader<>))
                            continue;

                        var recordType = @interface.GenericTypeArguments[0];
                        var loader = Activator.CreateInstance(type);
                        RegisterLoader(recordType, loader);
                    }
                }
            }
        }
    }
}
