using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;

public struct Datasheet
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property)]
    public class Sequence : System.Attribute
    {
        public int length;
    }

    public static List<T> Load<T>(string filename, int headerLines = 1) where T : new()
    {
        UnityEngine.Profiling.Profiler.BeginSample("Datasheet.Load");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        UnityEngine.Profiling.Profiler.BeginSample("File.ReadAllText");
        var fileSw = System.Diagnostics.Stopwatch.StartNew();
        string csv = File.ReadAllText(Application.streamingAssetsPath + "/" + filename);
        var fileReadMs = fileSw.ElapsedMilliseconds;
        UnityEngine.Profiling.Profiler.EndSample();

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
                throw new System.Exception("Field count mismatch " + typeof(T) + " (" + expectedFieldCount + " expected) at " + filename + ":" + (lineIndex + 1) + " (" + fields.Length + " fields)");

            if (lineIndex < headerLines)
                continue;

            try
            {
                T obj = new T();
                ReadObject(obj, members, fields);
                sheet.Add(obj);
            }
            catch (System.Exception e)
            {
                throw new System.Exception("Datasheet parsing error at line " + lineIndex + ", first field " + fields[0], e);
            }
        }
        Debug.Log("Load " + filename + " (" + sheet.Count + " items, elapsed " + stopwatch.Elapsed.Milliseconds + " ms, file read " + fileReadMs + " ms)");
        UnityEngine.Profiling.Profiler.EndSample();
        return sheet;
    }

    private static int CalcFieldCount(System.Type type)
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
                var seq = (Sequence)System.Attribute.GetCustomAttribute(member, typeof(Sequence), true);
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
            catch (System.Exception e)
            {
                throw new System.Exception("Datasheet parsing error at column " + (fieldIndex + 1) + " memberIndex " + memberIndex + " member " + member, e);
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
            var seq = (Sequence)System.Attribute.GetCustomAttribute(fi, typeof(Sequence), true);
            var array = (IList)System.Array.CreateInstance(elementType, seq.length);
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
                    object element = System.Activator.CreateInstance(elementType);
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

    static object CastValue(string value, System.Type type)
    {
        if (value == "" || value == "xxx")
            return null;

        if (type == typeof(bool))
        {
            if (value == "1")
                return true;
            
            if (value == "0")
                return false;

            throw new System.FormatException("Unable to cast '" + value + "' to bool");
        }

        if (type == typeof(string))
        {
            return value;
        }

        if (type == typeof(int))
        {
            return StringTools.ParseInt(value);
        }
        
        if (type == typeof(uint))
        {
            return StringTools.ParseUInt(value);
        }
        
        if (type == typeof(float))
        {
            return (float) System.Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        throw new System.FormatException("Unable to cast '" + value + "' to " + type);
    }
}
