using Coderama.Core.Models;

namespace Coderama.Core.Interfaces;

public interface IDocumentSerializer
{
    string Serialize(Document document, string format);
    Document? Deserialize(string content, string format);
    bool IsFormatSupported(string format);
}