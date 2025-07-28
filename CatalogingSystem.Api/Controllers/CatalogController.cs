using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CatalogingSystem.DTOs.Dtos;
using CatalogingSystem.Services.Interfaces;
using System.Text.Json;
using FluentValidation;

namespace CatalogingSystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _service;
    private readonly IValidator<CatalogItemDto> _catalogItemValidator;

    public CatalogController(ICatalogService service, IValidator<CatalogItemDto> catalogItemValidator)
    {
        _service = service;
        _catalogItemValidator = catalogItemValidator;
    }

    /// <summary>
    /// Retrieves paginated catalog items.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="size">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of catalog items with metadata.</returns>
    [HttpGet]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<PagedResultDto<CatalogItemDto>>> GetCatalogItems(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        return Ok(await _service.GetCatalogItems(page, size));
    }

    /// <summary>
    /// Searches catalog items by specified criteria with pagination.
    /// </summary>
    /// <param name="materialName">The material name to filter by.</param>
    /// <param name="authorName">The author name to filter by.</param>
    /// <param name="titleName">The title name to filter by.</param>
    /// <param name="genericClassification">The generic classification to filter by.</param>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="size">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of matching catalog items with metadata.</returns>
    [HttpGet("search")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<ActionResult<PagedResultDto<CatalogItemDto>>> GetCatalogItems(
        [FromQuery] long? expediente = null,
        [FromQuery] string? materialName = null,
        [FromQuery] string? authorName = null,
        [FromQuery] string? titleName = null,
        [FromQuery] string? genericClassification = null,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        if (expediente.HasValue)
        {
            var item = await _service.GetCatalogItem(expediente.Value);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(new PagedResultDto<CatalogItemDto>
            {
                Items = new[] { item },
                TotalItems = 1,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = 1
            });
        }

        var items = await _service.SearchCatalogItems(materialName, authorName, titleName, genericClassification, page, size);
        return Ok(items);
    }

    /// <summary>
    /// Deletes a catalog item and all related data by expediente.
    /// </summary>
    /// <param name="expediente">The expediente number.</param>
    /// <returns>NoContent if successful; otherwise, NotFound.</returns>
    [HttpDelete("{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> DeleteCatalogItem(long expediente)
    {
        try
        {
            var success = await _service.DeleteCatalogItem(expediente);
            return success ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    /// <summary>
    /// Exports a single catalog item as a JSON file by expediente.
    /// </summary>
    /// <param name="expediente">The expediente number.</param>
    /// <returns>A JSON file containing the catalog item, or NotFound if the expediente does not exist.</returns>
    [HttpGet("export/{expediente:long}")]
    [Authorize(Policy = "ArchivoAdminRead")]
    public async Task<IActionResult> ExportCatalog(long expediente)
    {
        var catalogItem = await _service.ExportCatalogItem(expediente);
        if (catalogItem == null)
        {
            return NotFound(new { message = $"No se encontró un expediente con el número {expediente}." });
        }

        var json = JsonSerializer.Serialize(catalogItem, new JsonSerializerOptions { WriteIndented = true });
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        return File(bytes, "application/json", $"catalog_item_{expediente}.json");
    }

    /// <summary>
    /// Imports catalog items from a JSON file, with an optional parameter to specify a new expediente.
    /// </summary>
    /// <param name="file">The JSON file containing catalog items.</param>
    /// <param name="nuevoExpediente">Optional new expediente number to use instead of the one in the JSON.</param>
    /// <returns>NoContent if successful; otherwise, BadRequest with validation errors.</returns>
    [HttpPost("import")]
    [Authorize(Policy = "ArchivoAdminWrite")]
    public async Task<IActionResult> ImportCatalog(IFormFile file, [FromQuery] long? nuevoExpediente = null)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No se proporcionó un archivo válido." });
        }

        if (!file.FileName.EndsWith(".json"))
        {
            return BadRequest(new { message = "El archivo debe ser un JSON." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            List<CatalogItemDto> catalogItems;

            try
            {
                catalogItems = await JsonSerializer.DeserializeAsync<List<CatalogItemDto>>(stream) ?? new List<CatalogItemDto>();
            }
            catch (JsonException)
            {
                stream.Position = 0;
                var singleItem = await JsonSerializer.DeserializeAsync<CatalogItemDto>(stream);
                catalogItems = singleItem != null
                    ? new List<CatalogItemDto> { singleItem }
                    : new List<CatalogItemDto>();
            }

            if (catalogItems == null || !catalogItems.Any())
            {
                return BadRequest(new { message = "El archivo JSON está vacío o no es válido." });
            }

            var validationErrors = new List<string>();
            foreach (var item in catalogItems)
            {
                var result = await _catalogItemValidator.ValidateAsync(item);
                if (!result.IsValid)
                {
                    validationErrors.AddRange(result.Errors.Select(e => $"Expediente {item.Expediente}: {e.ErrorMessage}"));
                }
            }

            if (validationErrors.Any())
            {
                return BadRequest(new { message = "Errores de validación", errors = validationErrors });
            }

            await _service.ImportCatalogItems(catalogItems, nuevoExpediente);
            return NoContent();
        }
        catch (JsonException)
        {
            return BadRequest(new { message = "El archivo JSON no tiene el formato correcto." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error al importar el catálogo: {ex.Message}" });
        }
    }
}