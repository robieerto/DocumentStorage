using Coderama.Core.Interfaces;
using Coderama.Core.Models;
using System.Text.Json;

namespace Coderama.Infrastructure.Serialization;

public class JsonDocumentSerializer : IDocumentFormatSerializer
{
    private readonly JsonSerializerOptions _options;

    public JsonDocumentSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public string Format => "application/json";

    public string Serialize(Document document)
    {
        return JsonSerializer.Serialize(document, _options);
    }

    public Document? Deserialize(string data)
    {
        return JsonSerializer.Deserialize<Document>(data, _options);
    }
}