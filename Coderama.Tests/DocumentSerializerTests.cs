using Coderama.Core.Interfaces;
using Coderama.Core.Models;
using Coderama.Infrastructure.Serialization;
using FluentAssertions;
using System.Text.Json;

namespace Coderama.Tests;

public class DocumentSerializerTests
{
    private readonly DocumentSerializer _serializer;
    private readonly Document _testDocument;

    public DocumentSerializerTests()
    {
        var factory = new DocumentSerializerFactory(new List<IDocumentFormatSerializer>
        {
            new JsonDocumentSerializer(),
            new XmlDocumentSerializer(),
            new MessagePackDocumentSerializer()
        });
        _serializer = new DocumentSerializer(factory);

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
                    ""version"": ""1.1""
                },
                ""content"": {
                    ""sections"": [
                        { ""id"": ""1"", ""text"": ""Section 1"" },
                        { ""id"": ""2"", ""text"": ""Section 2"" }
                    ],
                    ""settings"": {
                        ""enabled"": ""true"",
                        ""maxItems"": ""100""
                    }
                }
            }")
        };
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/xml")]
    [InlineData("application/x-msgpack")]
    public void SerializeAndDeserialize_PreservesAllData(string format)
    {
        // Act
        var serialized = _serializer.Serialize(_testDocument, format);
        var deserialized = _serializer.Deserialize(serialized, format);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(_testDocument.Id);
        deserialized.Tags.Should().BeEquivalentTo(_testDocument.Tags);

        // Verify nested data structure
        deserialized.Data.Should().NotBeNull();
        var deserializedJson = JsonSerializer.Serialize(deserialized.Data);
        var testDocumentJson = JsonSerializer.Serialize(_testDocument.Data);
        deserializedJson.Should().BeEquivalentTo(testDocumentJson);
    }

    [Theory]
    [InlineData("application/json")]
    [InlineData("application/xml")]
    [InlineData("application/x-msgpack")]
    public void Serialize_WithEmptyDocument_WorksCorrectly(string format)
    {
        // Arrange
        var emptyDoc = new Document
        {
            Id = "empty-doc",
            Tags = new List<string>(),
            Data = JsonSerializer.Deserialize<JsonElement>("{}")
        };

        // Assert for each format
        var serialized = _serializer.Serialize(emptyDoc, format);
        var deserialized = _serializer.Deserialize(serialized, format);

        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(emptyDoc.Id);
        deserialized.Tags.Should().BeEmpty();
    }

    [Fact]
    public void IsFormatSupported_ReturnsCorrectResults()
    {
        // Act & Assert
        _serializer.IsFormatSupported("application/json").Should().BeTrue();
        _serializer.IsFormatSupported("application/xml").Should().BeTrue();
        _serializer.IsFormatSupported("application/x-msgpack").Should().BeTrue();
        _serializer.IsFormatSupported("unsupported-format").Should().BeFalse();
    }

    [Fact]
    public void Serialize_WithUnsupportedFormat_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _serializer.Serialize(_testDocument, "unsupported-format");
        act.Should().Throw<ArgumentException>().WithMessage("*Unsupported format*");
    }

    [Fact]
    public void Deserialize_WithUnsupportedFormat_ThrowsArgumentException()
    {
        // Act & Assert
        var act = () => _serializer.Deserialize("some-data", "unsupported-format");
        act.Should().Throw<ArgumentException>().WithMessage("*Unsupported format*");
    }
}