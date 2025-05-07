using Coderama.Core.Interfaces;
using Coderama.Core.Models;
using Newtonsoft.Json;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Coderama.Infrastructure.Serialization;

public class XmlDocumentSerializer : IDocumentFormatSerializer
{
    private readonly XmlSerializer _serializer;
    private readonly JsonSerializerOptions _jsonOptions;

    public XmlDocumentSerializer()
    {
        _serializer = new XmlSerializer(typeof(Document));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public string Format => "application/xml";

    public string Serialize(Document document)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(document);
        var node = JsonConvert.DeserializeXNode(json, "Document");
        return node?.ToString() ?? string.Empty;
    }

    public Document? Deserialize(string xml)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        var json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true);
        return System.Text.Json.JsonSerializer.Deserialize<Document>(json, _jsonOptions);
    }
}