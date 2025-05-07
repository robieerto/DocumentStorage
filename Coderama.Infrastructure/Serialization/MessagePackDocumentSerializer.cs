using Coderama.Core.Interfaces;
using Coderama.Core.Models;
using MessagePack;
using System.Text.Json;

namespace Coderama.Infrastructure.Serialization;

public class MessagePackDocumentSerializer : IDocumentFormatSerializer
{
    private readonly MessagePackSerializerOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    public MessagePackDocumentSerializer()
    {
        _options = MessagePackSerializerOptions.Standard;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public string Format => "application/x-msgpack";

    public string Serialize(Document document)
    {
        var json = JsonSerializer.Serialize(document);
        return Convert.ToBase64String(MessagePackSerializer.ConvertFromJson(json, _options));
    }

    public Document? Deserialize(string data)
    {
        var bytes = Convert.FromBase64String(data);
        var json = MessagePackSerializer.ConvertToJson(bytes, _options);
        return JsonSerializer.Deserialize<Document>(json, _jsonOptions);
    }
}