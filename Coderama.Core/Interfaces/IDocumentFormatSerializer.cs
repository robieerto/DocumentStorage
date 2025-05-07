using Coderama.Core.Models;

namespace Coderama.Core.Interfaces;

public interface IDocumentFormatSerializer
{
    string Format { get; }
    string Serialize(Document document);
    Document? Deserialize(string data);
} 