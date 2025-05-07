using Coderama.Core.Models;
using Coderama.Infrastructure.Storage;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Coderama.Tests;

public class InMemoryDocumentStorageTests
{
    private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly InMemoryDocumentStorage _storage;
    private readonly Document _testDocument;

    public InMemoryDocumentStorageTests()
    {
        _storage = new InMemoryDocumentStorage(_cache);

        // Create a test document with nested data
        _testDocument = new Document
        {
            Id = "test-doc-1",
            Tags = new List<string> { "test", "sample" },
            Data = JsonSerializer.Deserialize<JsonElement>(@"{
                ""title"": ""Test Document"",
                ""metadata"": {
                    ""created"": ""2024-01-01"",
                    ""author"": ""test-user"",
                    ""version"": 1.0
                },
                ""content"": {
                    ""sections"": [
                        { ""id"": 1, ""text"": ""Section 1"" },
                        { ""id"": 2, ""text"": ""Section 2"" }
                    ],
                    ""settings"": {
                        ""enabled"": true,
                        ""maxItems"": 100
                    }
                }
            }")
        };
    }

    [Fact]
    public async Task StoreAndRetrieve_WorksCorrectly()
    {
        // Act
        await _storage.StoreAsync(_testDocument);
        var retrieved = await _storage.GetByIdAsync(_testDocument.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(_testDocument.Id);
        retrieved.Tags.Should().BeEquivalentTo(_testDocument.Tags);

        var retrievedData = JsonSerializer.Serialize(retrieved.Data);
        var originalData = JsonSerializer.Serialize(_testDocument.Data);
        retrievedData.Should().BeEquivalentTo(originalData);
    }

    [Fact]
    public async Task StoreAndUpdate_WorksCorrectly()
    {
        // Arrange
        await _storage.StoreAsync(_testDocument);

        // Act
        var updatedDoc = new Document
        {
            Id = _testDocument.Id,
            Tags = new List<string> { "updated", "test" },
            Data = JsonSerializer.Deserialize<JsonElement>(@"{
                ""title"": ""Updated Document"",
                ""metadata"": {
                    ""created"": ""2024-01-01"",
                    ""modified"": ""2024-01-02"",
                    ""author"": ""test-user"",
                    ""version"": 2.0
                }
            }")
        };

        await _storage.StoreAsync(updatedDoc);
        var retrieved = await _storage.GetByIdAsync(_testDocument.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(updatedDoc.Id);
        retrieved.Tags.Should().BeEquivalentTo(updatedDoc.Tags);

        var updatedData = JsonSerializer.Serialize(updatedDoc.Data);
        var retrievedData = JsonSerializer.Serialize(retrieved.Data);
        retrievedData.Should().BeEquivalentTo(updatedData);
    }

    [Fact]
    public async Task StoreAndDelete_WorksCorrectly()
    {
        // Arrange
        await _storage.StoreAsync(_testDocument);

        // Act
        await _storage.DeleteAsync(_testDocument.Id);
        var retrieved = await _storage.GetByIdAsync(_testDocument.Id);

        // Assert
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task Retrieve_NonExistentDocument_ReturnsNull()
    {
        // Act
        var retrieved = await _storage.GetByIdAsync("non-existent-id");

        // Assert
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task Store_MultipleDocuments_WorksCorrectly()
    {
        // Arrange
        var doc2 = new Document
        {
            Id = "test-doc-2",
            Tags = new List<string> { "test", "second" },
            Data = JsonSerializer.Deserialize<JsonElement>(@"{
                ""title"": ""Second Document"",
                ""metadata"": {
                    ""created"": ""2024-01-02"",
                    ""author"": ""test-user-2""
                }
            }")
        };

        // Act
        await _storage.StoreAsync(_testDocument);
        await _storage.StoreAsync(doc2);

        var retrieved1 = await _storage.GetByIdAsync(_testDocument.Id);
        var retrieved2 = await _storage.GetByIdAsync(doc2.Id);

        // Assert
        retrieved1.Should().NotBeNull();
        retrieved1!.Id.Should().Be(_testDocument.Id);

        retrieved2.Should().NotBeNull();
        retrieved2!.Id.Should().Be(doc2.Id);
    }

    [Fact]
    public async Task Store_WithEmptyDocument_WorksCorrectly()
    {
        // Arrange
        var emptyDoc = new Document
        {
            Id = "empty-doc",
            Tags = new List<string>(),
            Data = JsonSerializer.Deserialize<JsonElement>("{}")
        };

        // Act
        await _storage.StoreAsync(emptyDoc);
        var retrieved = await _storage.GetByIdAsync(emptyDoc.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(emptyDoc.Id);
        retrieved.Tags.Should().BeEmpty();
        retrieved.Data.ValueKind.Should().Be(JsonValueKind.Object);
    }

    [Fact]
    public async Task Store_WithComplexNestedStructure_WorksCorrectly()
    {
        // Arrange
        var complexDoc = new Document
        {
            Id = "complex-doc",
            Tags = new List<string> { "complex", "nested" },
            Data = JsonSerializer.Deserialize<JsonElement>(@"{
                ""config"": {
                    ""api"": {
                        ""endpoints"": [
                            { ""path"": ""/api/v1"", ""method"": ""GET"" },
                            { ""path"": ""/api/v2"", ""method"": ""POST"" }
                        ],
                        ""security"": {
                            ""enabled"": true,
                            ""timeout"": 30,
                            ""headers"": {
                                ""Authorization"": ""Bearer"",
                                ""Content-Type"": ""application/json""
                            }
                        }
                    }
                }
            }")
        };

        // Act
        await _storage.StoreAsync(complexDoc);
        var retrieved = await _storage.GetByIdAsync(complexDoc.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(complexDoc.Id);
        retrieved.Tags.Should().BeEquivalentTo(complexDoc.Tags);

        var originalData = JsonSerializer.Serialize(complexDoc.Data);
        var retrievedData = JsonSerializer.Serialize(retrieved.Data);
        retrievedData.Should().BeEquivalentTo(originalData);
    }
}