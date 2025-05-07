using Coderama.Core.Interfaces;
using Coderama.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Coderama.API.Controllers;

[ApiController]
[Route("documents")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentStorage _storage;
    private readonly IDocumentSerializer _serializer;

    public DocumentsController(IDocumentStorage storage, IDocumentSerializer serializer)
    {
        _storage = storage;
        _serializer = serializer;
    }

    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> CreateDocument([FromBody] Document document)
    {
        if (string.IsNullOrEmpty(document.Id))
        {
            return BadRequest("Document ID is required");
        }

        await _storage.StoreAsync(document);
        return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDocument(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Document ID is required");
        }

        var document = await _storage.GetByIdAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        var accept = Request.GetTypedHeaders().Accept;
        if (accept.Count == 0)
        {
            return Ok(document);
        }

        var supportedFormat = accept
            .Select(x => x.MediaType.ToString())
            .FirstOrDefault(format => _serializer.IsFormatSupported(format) || format == "*/*");

        if (supportedFormat == null)
        {
            return StatusCode(406, $"Unsupported format");
        }
        if (supportedFormat == "*/*")
        {
            supportedFormat = "application/json";
        }

        var serialized = _serializer.Serialize(document, supportedFormat);
        return Content(serialized, supportedFormat);
    }

    [HttpPut("{id}")]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateDocument(string id, [FromBody] Document document)
    {
        if (id != document.Id)
        {
            return BadRequest("Document ID mismatch");
        }

        var success = await _storage.UpdateAsync(id, document);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Document ID is required");
        }

        var success = await _storage.DeleteAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}