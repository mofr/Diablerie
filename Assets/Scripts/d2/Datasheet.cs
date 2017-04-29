using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;

public struct Datasheet<T> where T : new()
{
    public List<T> rows;

    public static Datasheet<T> Load(string filename, int headerLines = 1)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        string csv = File.ReadAllText(Application.streamingAssetsPath + "/" + filename);
        MemberInfo[] members = FormatterServices.GetSerializableMembers(typeof(T));
        int expectedFieldCount = 0;
        T dummy = new T();
        foreach (MemberInfo member in members)
        {
            FieldInfo fi = (FieldInfo)member;
            if (fi.FieldType.IsArray)
            {
                expectedFieldCount += ((System.Collections.IList)fi.GetValue(dummy)).Count;
            }
            else
            {
                expectedFieldCount += 1;
            }
        }
        Datasheet<T> sheet = new Datasheet<T>();
        sheet.rows = new List<T>();
        var lines = csv.Split('\n');
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
                T obj = ReadLine(fields, members);
                sheet.rows.Add(obj);
            }
            catch (System.Exception)
            {
                throw new System.Exception("Datasheet parsing error at " + filename + ":" + (lineIndex + 1));
            }
        }
        Debug.Log("Load " + filename + " (" + sheet.rows.Count + " items, elapsed " + stopwatch.Elapsed.Milliseconds + " ms)");
        return sheet;
    }

    static T ReadLine(string[] fields, MemberInfo[] members)
    {
        T obj = new T();
        int fieldIndex = 0;
        for (int memberIndex = 0; memberIndex < members.Length; ++memberIndex)
        {
            MemberInfo member = members[memberIndex];
            try
            {
                FieldInfo fi = (FieldInfo)member;
                fieldIndex = ReadMember(obj, fi, fields, fieldIndex);
            }
            catch (System.Exception)
            {
                throw new System.Exception("Datasheet parsing error at column " + (fieldIndex + 1) + " memberIndex " + memberIndex + " member " + member);
            }
        }

        return obj;
    }

    static int ReadMember(T obj, FieldInfo fi, string[] fields, int fieldIndex)
    {
        if (fi.FieldType.IsArray)
        {
            var elementType = fi.FieldType.GetElementType();
            var array = (System.Collections.IList)fi.GetValue(obj);
            for (int i = 0; i < array.Count; ++i)
            {
                array[i] = CastValue(fields[fieldIndex], elementType, array[i]);
                ++fieldIndex;
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
