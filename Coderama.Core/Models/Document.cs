using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Coderama.Core.Models;

public class Document
{
    [Required]
    public string Id { get; set; } = string.Empty;
    [Required]
    public List<string> Tags { get; set; } = [];
    [Required]
    public JsonElement Data { get; set; }
}
