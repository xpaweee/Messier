using System.Text.Json;
using System.Text.Json.Serialization;
using Messier.Interfaces;

namespace Messier.JsonSerializer;

public class JsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions? _options;

    public JsonSerializer()
    {
        _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
    }

    public string Serialize<TType>(TType value)
        => System.Text.Json.JsonSerializer.Serialize(value, _options);

    public TType? Deserialize<TType>(string value)
        => System.Text.Json.JsonSerializer.Deserialize<TType>(value, _options);
    
    public object? Deserialize(string value, Type type)
        => System.Text.Json.JsonSerializer.Deserialize(value,type, _options);
}