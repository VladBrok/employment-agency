using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using EmploymentAgency.Models;

namespace EmploymentAgency.Helpers;

public class IdentifiableConverter<T> : JsonConverter<T>
    where T : IIDentifiable
{
    public override T Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options)
    {
        return (T)JsonSerializer.Deserialize(ref reader, type, options)!;
    }

    public override void Write(
        Utf8JsonWriter writer,
        T obj,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        var props = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        for (int i = 0; i < props.Length; i++)
        {
            if (!ShouldIgnore(props[i].Name))
            {
                WriteProperty(writer, options, obj, props[i]);
            }
        }

        writer.WriteEndObject();
    }

    private bool ShouldIgnore(string propName)
    {
        return propName.EndsWith("Id") && propName.Length > 3 || propName == "LazyLoader";
    }

    private void WriteProperty(
        Utf8JsonWriter writer,
        JsonSerializerOptions options,
        T obj,
        PropertyInfo prop)
    {
        writer.WritePropertyName(prop.Name);
        object? value = prop.GetValue(obj);
        JsonSerializer.Serialize(writer, value, value?.GetType() ?? typeof(object), options);
    }
}
