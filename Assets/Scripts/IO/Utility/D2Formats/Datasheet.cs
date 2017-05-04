using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;

public struct Datasheet
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class Sequence : System.Attribute
    {
        public int length;
    }

    public static List<T> Load<T>(string filename, int headerLines = 1) where T : new()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        string csv = File.ReadAllText(Application.streamingAssetsPath + "/" + filename);
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

            T obj = new T();
            ReadObject(obj, members, fields);
            sheet.Add(obj);
        }
        Debug.Log("Load " + filename + " (" + sheet.Count + " items, elapsed " + stopwatch.Elapsed.Milliseconds + " ms)");
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
            catch (System.Exception)
            {
                throw new System.Exception("Datasheet parsing error at column " + (fieldIndex + 1) + " memberIndex " + memberIndex + " member " + member);
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
                    array[i] = CastValue(fields[fieldIndex], elementType, array[i]);
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
            var value = CastValue(fields[fieldIndex], fi.FieldType, fi.GetValue(obj));
            fi.SetValue(obj, value);
            ++fieldIndex;
        }

        return fieldIndex;
    }

    static object CastValue(string value, System.Type type, object defaultValue)
    {
        if (value == "" || value == "xxx")
            return defaultValue;

        if (type == typeof(bool))
        {
            if (value == "1")
                return true;
            else if (value == "0")
                return false;
            else
                throw new System.FormatException("Unable to cast '" + value + "' to bool");
        }
        else
        {
            return System.Convert.ChangeType(value, type);
        }
    }
}
