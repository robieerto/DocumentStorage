using Coderama.Core.Interfaces;
using Coderama.Core.Models;

namespace Coderama.Infrastructure.Serialization;

public class DocumentSerializer : IDocumentSerializer
{
    private readonly DocumentSerializerFactory _factory;

    public DocumentSerializer(DocumentSerializerFactory factory)
    {
        _factory = factory;
    }

    public string Serialize(Document document, string format)
    {
        var serializer = _factory.GetSerializer(format);
        return serializer.Serialize(document);
    }

    public Document? Deserialize(string data, string format)
    {
        var serializer = _factory.GetSerializer(format);
        return serializer.Deserialize(data);
    }

    public bool IsFormatSupported(string format)
    {
        return _factory.HasSerializer(format);
    }
}
