using Coderama.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Coderama.Infrastructure.Serialization;

public class DocumentSerializerFactory
{
    private readonly Dictionary<string, IDocumentFormatSerializer> _serializers;

    public DocumentSerializerFactory(IEnumerable<IDocumentFormatSerializer> serializers)
    {
        _serializers = serializers.ToDictionary(s => s.Format);
    }

    public IDocumentFormatSerializer GetSerializer(string format)
    {
        if (!_serializers.TryGetValue(format, out var serializer))
        {
            throw new ArgumentException($"Unsupported format: {format}");
        }

        return serializer;
    }

    public bool HasSerializer(string format)
    {
        return _serializers.ContainsKey(format);
    }

    public static void RegisterSerializers(IServiceCollection services)
    {
        services.AddSingleton<IDocumentFormatSerializer, JsonDocumentSerializer>();
        services.AddSingleton<IDocumentFormatSerializer, XmlDocumentSerializer>();
        services.AddSingleton<IDocumentFormatSerializer, MessagePackDocumentSerializer>();
    }
}