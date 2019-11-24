using System;
using System.Collections;
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

    public interface Loader<T>
    {
        void LoadRecord(ref T record, Stream stream);
    }

    private static Dictionary<Type, object> loaders = new Dictionary<Type, object>();

    public static void RegisterLoader<T>(Loader<T> loader)
    {
        loaders.Add(typeof(T), loader);
    }
    
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

        MemberInfo[] members = FormatterServices.GetSerializableMembers(typeof(T));
        int expectedFieldCount = CalcFieldCount(typeof(T));
        var lines = csv.Split('\n');
        var sheet = new List<T>(lines.Length);
        for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
        {
            string line = lines[lineIndex];
            line = line.Replace("\r", "");
            if (line.Length == 0)
                continue;

            var fields = line.Split('\t');

            if (fields.Length != expectedFieldCount)
                throw new Exception("Field count mismatch " + typeof(T) + " (" + expectedFieldCount + " expected) at " + filename + ":" + (lineIndex + 1) + " (" + fields.Length + " fields)");

            if (lineIndex < headerLines)
                continue;

            try
            {
                T obj = new T();
                if (loader != null)
                {
                    var stream = new Stream(fields);
                    loader.LoadRecord(ref obj, stream);
                }
                else
                {
                    ReadObject(obj, members, fields);
                }
                sheet.Add(obj);
            }
            catch (Exception e)
            {
                throw new Exception("Datasheet parsing error at line " + lineIndex + ", first field " + fields[0], e);
            }
        }
        Debug.Log("Load " + filename + " (" + sheet.Count + " items, elapsed " + stopwatch.Elapsed.Milliseconds + " ms, file read " + fileReadMs + " ms)");
        Profiler.EndSample();
        return sheet;
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

    static int ReadObject(object obj, MemberInfo[] members, string[] fields, int fieldIndex = 0)
    {
        for (int memberIndex = 0; memberIndex < members.Length; ++memberIndex)
        {
            MemberInfo member = members[memberIndex];
            try
            {
                fieldIndex = ReadMember(obj, member, fields, fieldIndex);
            }
            catch (Exception e)
            {
                throw new Exception("Datasheet parsing error at column " + (fieldIndex + 1) + " memberIndex " + memberIndex + " member " + member, e);
            }
        }
        return fieldIndex;
    }

    static int ReadMember(object obj, MemberInfo member, string[] fields, int fieldIndex)
    {
        FieldInfo fi = (FieldInfo)member;
        if (fi.FieldType.IsArray)
        {
            var elementType = fi.FieldType.GetElementType();
            var seq = (Sequence)Attribute.GetCustomAttribute(fi, typeof(Sequence), true);
            var array = (IList)Array.CreateInstance(elementType, seq.length);
            fi.SetValue(obj, array);
            var elementMembers = FormatterServices.GetSerializableMembers(elementType);
            for (int i = 0; i < array.Count; ++i)
            {
                if (elementMembers.Length == 0)
                {
                    object value = CastValue(fields[fieldIndex], elementType);
                    if (value != null)
                        array[i] = value;
                    ++fieldIndex;
                }
                else
                {
                    object element = Activator.CreateInstance(elementType);
                    fieldIndex = ReadObject(element, elementMembers, fields, fieldIndex);
                    array[i] = element;
                }
            }
        }
        else
        {
            object value = CastValue(fields[fieldIndex], fi.FieldType);
            if (value != null)
                fi.SetValue(obj, value);
            ++fieldIndex;
        }

        return fieldIndex;
    }

    static object CastValue(string value, Type type)
    {
        if (value == "" || value == "xxx")
            return null;

        if (type == typeof(bool))
        {
            return ParseBool(value);
        }

        if (type == typeof(string))
        {
            return value;
        }

        if (type == typeof(int))
        {
            return ParseInt(value);
        }
        
        if (type == typeof(uint))
        {
            return ParseUInt(value);
        }
        
        if (type == typeof(float))
        {
            return (float) Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        throw new FormatException("Unable to cast '" + value + "' to " + type);
    }

    public struct Stream
    {
        private string[] values;
        private int index;
        
        public Stream(string[] values)
        {
            this.values = values;
            index = 0;
        }

        public string NextString()
        {
            return values[index++];
        }
    }

    public static void Parse(string value, ref int result)
    {
        if (value == "" || value == "xxx")
            return;
        result = ParseInt(value);
    }

    public static void Parse(string value, ref uint result)
    {
        if (value == "" || value == "xxx")
            return;
        result = ParseUInt(value);
    }

    public static void Parse(string value, ref string result)
    {
        if (value == "" || value == "xxx")
            return;
        result = value;
    }

    public static void Parse(string value, ref bool result)
    {
        if (value == "" || value == "xxx")
            return;
        result = ParseBool(value);
    }

    public static void Parse(string value, ref float result)
    {
        if (value == "" || value == "xxx")
            return;
        result = (float) Convert.ToDouble(value, CultureInfo.InvariantCulture);
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
