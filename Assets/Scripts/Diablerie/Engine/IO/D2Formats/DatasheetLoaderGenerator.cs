using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using Scriban.Runtime;

namespace Diablerie.Engine.IO.D2Formats
{
    public class DatasheetLoaderGenerator
    {
        private class TypeInfo
        {
            public Type type;
            public Field[] fields;

            public TypeInfo(Type type)
            {
                this.type = type;
                if (type.IsPrimitive)
                {
                    fields = new Field[0];
                }
                else
                {
                    MemberInfo[] members = FormatterServices.GetSerializableMembers(type);
                    fields = new Field[members.Length];
                    for (int i = 0; i < members.Length; ++i)
                    {
                        fields[i] = new Field(members[i]);
                    }
                }
            }

            public bool IsCustom
            {
                get
                {
                    if (type == typeof(int))
                        return false;
                    if (type == typeof(uint))
                        return false;
                    if (type == typeof(bool))
                        return false;
                    if (type == typeof(float))
                        return false;
                    if (type == typeof(string))
                        return false;
                    return true;
                }
            }

            public string Declaration
            {
                get
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
                    if (type.IsNested)
                        return type.ReflectedType.Name + "." + type.Name;
                    return type.Name;
                }
            }

            public string Name
            {
                get
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
                    if (type.IsNested)
                        return type.ReflectedType.Name + type.Name;
                    return type.Name;
                }
            }

            public void CollectDependencies(ISet<TypeInfo> dependencies)
            {
                foreach (var field in fields)
                {
                    CollectDependencies(dependencies, field);
                }
            }
        
            private void CollectDependencies(ISet<TypeInfo> dependencies, Field field)
            {
                if (field.isArray)
                {
                    CollectDependencies(dependencies, field.element);
                }
                else
                {
                    CollectDependencies(dependencies, field.typeInfo);
                }
            }
        
            private void CollectDependencies(ISet<TypeInfo> dependencies, TypeInfo from)
            {
                if (from.IsCustom)
                    dependencies.Add(from);
                foreach (var field in from.fields)
                {
                    CollectDependencies(dependencies, field);
                }
            }

            public override bool Equals(object obj)
            {
                var typeInfo = obj as TypeInfo;
                return typeInfo != null && type == typeInfo.type;
            }

            public override int GetHashCode()
            {
                return type.GetHashCode();
            }
        }
    
        private class Field
        {
            public string name;
            public TypeInfo typeInfo;
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
                    typeInfo = new TypeInfo(fieldInfo.FieldType);
                }
            }
        }
    
        private static readonly Scriban.Template template = Scriban.Template.Parse(@"
// It's generated file. DO NOT MODIFY IT!
class {{ type.name }}Loader : Datasheet.Loader<{{ type.declaration }}>
{
    {{~ for dependency in dependencies ~}}
    private {{ dependency.name }}Loader {{ dependency.name | string.downcase }}loader = new {{ dependency.name }}Loader();
    {{~ end ~}}

    public void LoadRecord(ref {{ type.declaration }} record, Datasheet.Stream stream)
    {
        {{~ for field in type.fields ~}}
            {{~ if field.is_array ~}}
                record.{{ field.name }} = new {{ field.element.declaration }}[{{ field.array_size }}];
                {{~ for item_index in 0..(field.array_size-1) ~}}
                    {{~ if field.element.is_custom ~}}
                    {{ field.element.name | string.downcase }}loader.LoadRecord(ref record.{{ field.name }}[{{ item_index }}], stream);
                    {{~ else ~}}
                    stream.Read(ref record.{{ field.name }}[{{ item_index }}]);
                    {{~ end ~}}
                {{~ end ~}}
            {{~ else ~}}
                stream.Read(ref record.{{ field.name }});
            {{~ end ~}}
        {{~ end ~}}
    }
}
");
    
        public string Generate(Type recordType)
        {
            var typeInfo = new TypeInfo(recordType);
            ISet<TypeInfo> dependencies = new HashSet<TypeInfo>();
            typeInfo.CollectDependencies(dependencies);
            var scriptObject = new ScriptObject();
            scriptObject["type"] = typeInfo;
            scriptObject["dependencies"] = dependencies;
            var context = new Scriban.TemplateContext();
            context.PushGlobal(scriptObject);
            return template.Render(context);
        }
    
        public string GenerateToDirectory(Type recordType, string directory)
        {
            string loaderFileContents = Generate(recordType);
            string filename = directory + GetLoaderFilenameForType(recordType) + "Loader.cs"; 
            File.WriteAllText(filename, loaderFileContents);
            return filename;
        }

        private static string GetLoaderFilenameForType(Type type)
        {
            if (type.IsNested)
                return type.ReflectedType.Name + type.Name;
            return type.Name;
        }
    }
}
