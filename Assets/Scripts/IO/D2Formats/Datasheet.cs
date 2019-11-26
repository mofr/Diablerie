using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

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

        int expectedFieldCount = CalcFieldCount(typeof(T));
        var lines = csv.Split('\n');
        var sheet = new List<T>(lines.Length);
        for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
        {
            if (lineIndex < headerLines)
                continue;
            
            string line = lines[lineIndex];
            line = line.Replace("\r", "");
            if (line.Length == 0)
                continue;

            var fieldCount = CalcFieldCount(line);
            if (fieldCount != expectedFieldCount)
                throw new Exception("Field count " + fieldCount + ", expected " + expectedFieldCount + ") at " + filename + ":" + (lineIndex + 1));

            try
            {
                var stream = new Stream(line);
                T obj = new T();
                loader.LoadRecord(ref obj, stream);
                sheet.Add(obj);
            }
            catch (Exception e)
            {
                throw new Exception("Datasheet parsing error at line " + lineIndex + ", first field " + line.Substring(0, 16), e);
            }
        }
        Debug.Log("Load " + filename + " (" + sheet.Count + " items, elapsed " + stopwatch.Elapsed.Milliseconds + " ms, file read " + fileReadMs + " ms)");
        Profiler.EndSample();
        return sheet;
    }

    private static int CalcFieldCount(string line)
    {
        int count = 0;
        for (int i = 0; i < line.Length; ++i)
            if (line[i] == '\t')
                count++;
        return count + 1;
    }

    private static int CalcFieldCount(Type type)
    {
        MemberInfo[] members = FormatterServices.GetSerializableMembers(type);
        if (members.Length == 0)
            return 1;

        int fieldCount = 0;
        foreach (MemberInfo member in members)
        {
            FieldInfo fi = (FieldInfo)member;
            if (fi.FieldType.IsArray)
            {
                var seq = (Sequence)Attribute.GetCustomAttribute(member, typeof(Sequence), true);
                var elementType = fi.FieldType.GetElementType();
                int elementFieldCount = CalcFieldCount(elementType);
                fieldCount += seq.length * elementFieldCount;
            }
            else
            {
                fieldCount += 1;
            }
        }

        return fieldCount;
    }

    public class Stream
    {
        private string line;
        private int index;
        
        public Stream(string line)
        {
            this.line = line;
            index = 0;
        }

        public string NextString()
        {
            int endIndex = index;
            while (endIndex < line.Length && line[endIndex] != '\t')
                endIndex++;
            string result = line.Substring(index, endIndex - index);
            index = endIndex + 1;
            return result;
        }

        public void Read(ref int result)
        {
            var value = NextString();
            if (value == "" || value == "xxx")
                return;
            result = ParseInt(value);
        }

        public void Read(ref uint result)
        {
            var value = NextString();
            if (value == "" || value == "xxx")
                return;
            result = ParseUInt(value);
        }
        
        public void Read(ref string result)
        {
            var value = NextString();
            if (value == "" || value == "xxx")
                return;
            result = value;
        }

        public void Read(ref bool result)
        {
            var value = NextString();
            if (value == "" || value == "xxx")
                return;
            result = ParseBool(value);
        }

        public void Read(ref float result)
        {
            var value = NextString();
            if (value == "" || value == "xxx")
                return;
            result = (float) Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
    }

    public static int ParseInt(string str)
    {
        if (str.Length == 0)
            return 0;
        int result;
        int sign;
        if (str[0] == '-')
        {
            sign = -1;
            result = 0;
        }
        else
        {
            sign = 1;
            result = str[0] - '0';
        }

        for (int i = 1; i < str.Length; i++)
        {
            result = result * 10 + (str[i] - '0');
        }

        return result * sign;
    }

    public static uint ParseUInt(string str)
    {
        uint result = 0;
        for (int i = 0; i < str.Length; i++)
        {
            result = result * 10 + (uint)(str[i] - '0');
        }
        return result;
    }

    public static bool ParseBool(string value)
    {
        if (value == "1")
            return true;
            
        if (value == "0")
            return false;

        throw new System.FormatException("Unable to cast '" + value + "' to bool");
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
