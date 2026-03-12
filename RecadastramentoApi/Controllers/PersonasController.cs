using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecadastramentoApi.DTO;
using RecadastramentoApi.Export;
using RecadastramentoApi.Services;

namespace RecadastramentoApi.Controllers;

[ApiController]
[Authorize(Policy = "CityAccess")]
[Route("{cidade}/personas")]
public sealed class PersonasController(IPersonaService personaService, IPersonaExportService exportService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(PersonaBatchUpsertResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonaBatchUpsertResponseDto>> UpsertBatch(
        string cidade,
        [FromBody] List<PersonaUpsertDto> personas,
        CancellationToken cancellationToken)
    {
        _ = cidade;

        if (personas.Count == 0)
        {
            return BadRequest("The request body must contain at least one persona.");
        }

        var result = await personaService.UpsertBatchAsync(personas, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{cpf}")]
    [ProducesResponseType(typeof(PersonaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonaResponseDto>> GetByCpf(string cidade, string cpf, CancellationToken cancellationToken)
    {
        _ = cidade;

        var persona = await personaService.GetByCpfAsync(cpf, cancellationToken);
        if (persona is null)
        {
            return NotFound();
        }

        return Ok(persona);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PersonaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonaResponseDto>>> GetAll(
        string cidade,
        [FromQuery] PersonaFilterDto filter,
        CancellationToken cancellationToken)
    {
        _ = cidade;

        var personas = await personaService.GetAllAsync(filter, cancellationToken);
        return Ok(personas);
    }

    [HttpGet("export")]
    [ProducesResponseType(typeof(List<PersonaResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Export(
        string cidade,
        [FromQuery] string format,
        [FromQuery] PersonaFilterDto filter,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return BadRequest("Query param 'format' is required. Use json, csv or xlsx.");
        }

        var personas = await personaService.GetAllAsync(filter, cancellationToken);
        var normalizedFormat = format.Trim().ToLowerInvariant();

        return normalizedFormat switch
        {
            "json" => Ok(personas),
            "csv" => File(
                exportService.ExportCsv(personas),
                "text/csv; charset=utf-8",
                $"personas-{cidade}.csv"),
            "xlsx" => File(
                exportService.ExportXlsx(personas),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"personas-{cidade}.xlsx"),
            _ => BadRequest("Invalid format. Use json, csv or xlsx.")
        };
    }
}
