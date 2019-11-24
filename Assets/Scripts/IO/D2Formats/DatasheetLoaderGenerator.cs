using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Scriban.Runtime;

public class DatasheetLoaderGenerator
{
    private struct Field
    {
        public string name;
        public string type;
        
        public Field(MemberInfo memberInfo)
        {
            name = memberInfo.Name;
            var fieldInfo = (FieldInfo) memberInfo;
            if (fieldInfo.FieldType == typeof(int))
                type = "int";
            else if (fieldInfo.FieldType == typeof(uint))
                type = "uint";
            else if (fieldInfo.FieldType == typeof(bool))
                type = "bool";
            else if (fieldInfo.FieldType == typeof(float))
                type = "float";
            else
                type = "string";
        }
    }
    
    private static readonly Scriban.Template template = Scriban.Template.Parse(@"
// It's generated file. DO NOT MODIFY IT!
class {{ record_class }}Loader : Datasheet.Loader<{{ record_class }}>
{
    public void LoadRecord(ref {{ record_class }} record, string[] values)
    {
        int index = 0;
        {{~ for field in fields ~}}
            {{~ if field.type == 'int' ~}}
            record.{{ field.name }} = Datasheet.ParseInt(values[index++]);
            {{~ else if field.type == 'uint' ~}}
            record.{{ field.name }} = Datasheet.ParseUInt(values[index++]);
            {{~ else if field.type == 'bool' ~}}
            record.{{ field.name }} = Datasheet.ParseBool(values[index++]);
            {{~ else ~}}
            record.{{ field.name }} = values[index++];
            {{~ end ~}}
        {{~ end ~}}
    }
}
");
    
    public string Generate(Type recordType)
    {
        MemberInfo[] members = FormatterServices.GetSerializableMembers(recordType);
        var fields = new Field[members.Length];
        for (int i = 0; i < members.Length; ++i)
        {
            fields[i] = new Field(members[i]);
        }
        var scriptObject = new ScriptObject();
        scriptObject["record_class"] = recordType.Name;
        scriptObject["fields"] = fields;
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
