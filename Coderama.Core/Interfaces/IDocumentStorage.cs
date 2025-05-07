using Coderama.Core.Models;

namespace Coderama.Core.Interfaces;

public interface IDocumentStorage
{
    Task<Document?> GetByIdAsync(string id);
    Task<string> StoreAsync(Document document);
    Task<bool> UpdateAsync(string id, Document document);
    Task<bool> DeleteAsync(string id);
}