using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public struct Datasheet
{
    private static readonly char Separator = '\t';
    
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
        void LoadRecord(ref T record, Stream stream);
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
        var sheet = new List<T>(1024);
        int lineIndex = headerLines;
        StringSpan line = new StringSpan();
        while(splitter.ReadLine(ref line))
        {
            if (line.length == 0)
                continue;
            
            var stream = new Stream(line);
            T obj = new T();
            try
            {
                loader.LoadRecord(ref obj, stream);
            }
            catch (Exception e)
            {
                throw new Exception("Datasheet parsing error at line " + lineIndex + ": " + line.str.Substring(0, 32), e);
            }
            sheet.Add(obj);
        }
        Debug.Log("Load " + filename + " (" + sheet.Count + " items, elapsed " + stopwatch.Elapsed.Milliseconds + " ms, file read " + fileReadMs + " ms)");
        Profiler.EndSample();
        return sheet;
    }

    public class Stream
    {
        private string line;
        private int index;
        private int _end;
        
        public Stream(string line, int start = 0, int end = -1)
        {
            this.line = line;
            index = start;
            _end = end == -1 ? line.Length : end;
        }

        public Stream(StringSpan span) : this(span.str, span.index, span.index + span.length)
        {
        }

        public string NextString()
        {
            int endIndex = index;
            while (endIndex < _end && line[endIndex] != Separator)
                endIndex++;
            int length = endIndex - index;
            string result = null;
            if (length != 0)
                result = line.Substring(index, length);
            index = endIndex + 1;
            return result;
        }

        public void Read(ref int result)
        {
            if (index >= _end)
            {
                return;
            }
            if (line[index] == Separator)
            {
                index++;
                return;
            }
            int sign;
            char c = line[index++];
            if (c == '-')
            {
                sign = -1;
                result = 0;
            }
            else
            {
                sign = 1;
                result = c - '0';
            }

            while (index < _end && line[index] != Separator)
            {
                result = result * 10 + (line[index] - '0');
                index++;
            }

            result *= sign;
            index++; // skip tab
        }

        public void Read(ref uint result)
        {
            if (index >= _end)
            {
                return;
            }
            if (line[index] == Separator)
            {
                index++;
                return;
            }

            if (index < _end && line[index] != Separator)
                result = 0;
            while (index < _end && line[index] != Separator)
            {
                result = result * 10 + (uint)(line[index] - '0');
                index++;
            }

            index++; // skip tab
        }
        
        public void Read(ref string result)
        {
            var value = NextString();
            if (value == null || value == "xxx")
                return;
            result = value;
        }

        public void Read(ref bool result)
        {
            if (index >= _end)
            {
                return;
            }
            if (line[index] == Separator)
            {
                index++;
                return;
            }
            
            result = line[index] != '0';
            index += 2; // skip tab
        }

        public void Read(ref float result)
        {
            var value = NextString();
            if (value == null || value == "xxx")
                return;
            result = (float) Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
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
