using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Scriban.Runtime;

public class DatasheetLoaderGenerator
{
    private class TypeInfo
    {
        public string name;
        public Field[] fields;

        public TypeInfo(Type type)
        {
            name = GetTypeString(type);
            MemberInfo[] members = FormatterServices.GetSerializableMembers(type);
            fields = new Field[members.Length];
            for (int i = 0; i < members.Length; ++i)
            {
                fields[i] = new Field(members[i]);
            }
        }

        public static string GetTypeString(Type type)
        {
            if (type == typeof(int))
                return "int";
            if (type == typeof(uint))
                return "uint";
            if (type == typeof(bool))
                return "bool";
            if (type == typeof(float))
                return "float";
            if (type == typeof(string))
                return "string";
            return type.Name;
        }
    }
    
    private class Field
    {
        public string name;
        public string type;
        public bool isArray = false;
        public int arraySize = 0;
        public TypeInfo element = null;
        
        public Field(MemberInfo memberInfo)
        {
            name = memberInfo.Name;
            var fieldInfo = (FieldInfo) memberInfo;
            if (fieldInfo.FieldType.IsArray)
            {
                isArray = true;
                element = new TypeInfo(fieldInfo.FieldType.GetElementType());
                var seq = (Datasheet.Sequence)Attribute.GetCustomAttribute(fieldInfo, typeof(Datasheet.Sequence), true);
                arraySize = seq.length;
            }
            else
            {
                type = TypeInfo.GetTypeString(fieldInfo.FieldType);
            }
        }
    }
    
    private static readonly Scriban.Template template = Scriban.Template.Parse(@"
// It's generated file. DO NOT MODIFY IT!
class {{ type.name }}Loader : Datasheet.Loader<{{ type.name }}>
{
    public void LoadRecord(ref {{ type.name }} record, string[] values)
    {
        int index = 0;
        {{~ for field in type.fields ~}}
            {{~ if field.is_array ~}}
            record.{{ field.name }} = new {{ field.element.name }}[{{ field.array_size }}];
            index += {{ field.array_size }};  // TODO implement arrays
            {{~ else if field.type == 'int' ~}}
            if (values[index] != """")
                record.{{ field.name }} = Datasheet.ParseInt(values[index]);
            index++;
            {{~ else if field.type == 'uint' ~}}
            if (values[index] != """")
                record.{{ field.name }} = Datasheet.ParseUInt(values[index]);
            index++;
            {{~ else if field.type == 'bool' ~}}
            if (values[index] != """")
                record.{{ field.name }} = Datasheet.ParseBool(values[index]);
            index++;
            {{~ else ~}}
            if (values[index] != """")
                record.{{ field.name }} = values[index];
            index++;
            {{~ end ~}}
        {{~ end ~}}
    }
}
");
    
    public string Generate(Type recordType)
    {
        var scriptObject = new ScriptObject();
        scriptObject["type"] = new TypeInfo(recordType);
        var context = new Scriban.TemplateContext();
        context.PushGlobal(scriptObject);
        return template.Render(context);
    }
    
    public string GenerateToDirectory(Type recordType, string directory)
    {
        string loaderFileContents = Generate(recordType);
        string filename = directory + recordType.Name + "Loader.cs"; 
        File.WriteAllText(filename, loaderFileContents);
        return filename;
    }
}
