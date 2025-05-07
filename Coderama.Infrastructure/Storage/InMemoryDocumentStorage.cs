using Coderama.Core.Interfaces;
using Coderama.Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Coderama.Infrastructure.Storage;

public class InMemoryDocumentStorage : IDocumentStorage
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheOptions;
    private readonly Dictionary<string, Document> _storage = [];

    public InMemoryDocumentStorage(IMemoryCache cache)
    {
        _cache = cache;
        _cacheOptions = new MemoryCacheEntryOptions();
    }

    public Task<Document?> GetByIdAsync(string id)
    {
        if (_cache.TryGetValue<Document>(id, out var cachedDocument))
        {
            return Task.FromResult(cachedDocument);
        }

        if (_storage.TryGetValue(id, out var document))
        {
            _cache.Set(id, document, _cacheOptions);
            return Task.FromResult<Document?>(document);
        }

        return Task.FromResult<Document?>(null);
    }

    public Task<string> StoreAsync(Document document)
    {
        _storage[document.Id] = document;
        _cache.Set(document.Id, document, _cacheOptions);
        return Task.FromResult(document.Id);
    }

    public Task<bool> UpdateAsync(string id, Document document)
    {
        if (!_storage.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        _storage[id] = document;
        _cache.Set(id, document, _cacheOptions);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(string id)
    {
        if (!_storage.Remove(id))
        {
            return Task.FromResult(false);
        }

        _cache.Remove(id);
        return Task.FromResult(true);
    }
}