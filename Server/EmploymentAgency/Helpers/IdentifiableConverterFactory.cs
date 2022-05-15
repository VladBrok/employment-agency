using System.Text.Json;
using System.Text.Json.Serialization;
using EmploymentAgency.Models;

namespace EmploymentAgency.Helpers;

public class IdentifiableConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type type)
    {
        return type.IsAssignableTo(typeof(IIdentifiable));
    }

    public override JsonConverter CreateConverter(
        Type type,
        JsonSerializerOptions options)
    {
        return (JsonConverter)Activator.CreateInstance(
            typeof(IdentifiableConverter<>).MakeGenericType(type))!;
    }
}
