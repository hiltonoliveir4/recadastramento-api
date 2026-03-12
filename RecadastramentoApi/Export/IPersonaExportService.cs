using RecadastramentoApi.DTO;

namespace RecadastramentoApi.Export;

public interface IPersonaExportService
{
    byte[] ExportCsv(IEnumerable<PersonaResponseDto> personas);

    byte[] ExportXlsx(IEnumerable<PersonaResponseDto> personas);
}
