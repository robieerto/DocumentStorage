# DocumentStorage

Flexible document management system that supports multiple serialization formats and provides a RESTful API for document operations.

## Features

- **Flexible Document Structure**: Store documents with arbitrary nested data structures
- **Multiple Serialization Formats**:
  - JSON (application/json)
  - XML (application/xml)
  - MessagePack (application/x-msgpack)
- **RESTful API**: Full CRUD operations for document management
- **In-Memory Storage**: Fast document storage with caching support
- **Extensible Architecture**: Easy to add new serialization formats and storage providers

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code with C# extensions

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/Coderama.git
cd Coderama
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run --project Coderama.API
```

5. Access the Swagger at `https://localhost:5000/swagger/index.html`

## API Documentation

### Document Structure

```json
{
  "id": "string",
  "tags": ["string"],
  "data": {
    // Arbitrary nested JSON structure
  }
}
```

### Endpoints

#### Create Document
```http
POST /documents
Content-Type: application/json

{
  "id": "doc1",
  "tags": ["tag1", "tag2"],
  "data": {
    "title": "Sample Document",
    "metadata": {
      "created": "2024-01-01",
      "author": "user1"
    }
  }
}
```

#### Get Document
```http
GET /documents/{id}
Accept: application/json
```

#### Update Document
```http
PUT /documents/{id}
Content-Type: application/json

{
  "id": "doc1",
  "tags": ["tag1", "tag2", "tag3"],
  "data": {
    "title": "Updated Document",
    "metadata": {
      "created": "2024-01-01",
      "modified": "2024-01-02",
      "author": "user1"
    }
  }
}
```

#### Delete Document
```http
DELETE /documents/{id}
```

### Content Negotiation

The API supports multiple serialization formats. Specify the desired format using the `Accept` header:

- `application/json` - JSON format
- `application/xml` - XML format
- `application/x-msgpack` - MessagePack format

## Project Structure

```
Coderama/
├── Coderama.API/             # Web API project
├── Coderama.Core/            # Core domain models and interfaces
├── Coderama.Infrastructure/  # Implementation of interfaces
└── Coderama.Tests/           # Unit and integration tests
```

## Development

### Running Tests

```bash
dotnet test
```

### Adding New Serialization Formats

1. Create a new class implementing `IDocumentFormatSerializer`
2. Register the serializer in `DocumentSerializerFactory`
3. Add tests for the new format

Example:
```csharp
public class NewFormatSerializer : IDocumentFormatSerializer
{
    public string Format => "application/new-format";
    
    public string Serialize(Document document)
    {
        // Implementation
    }
    
    public Document? Deserialize(string data)
    {
        // Implementation
    }
}
```

### Adding New Storage Providers

1. Create a new class implementing `IDocumentStorage`
2. Register the storage provider in dependency injection
3. Add tests for the new provider
