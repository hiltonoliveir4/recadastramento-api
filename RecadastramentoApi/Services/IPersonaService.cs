using RecadastramentoApi.DTO;

namespace RecadastramentoApi.Services;

public interface IPersonaService
{
    Task<PersonaBatchUpsertResponseDto> UpsertBatchAsync(IReadOnlyList<PersonaUpsertDto> personas, CancellationToken cancellationToken = default);

    Task<PersonaResponseDto?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PersonaResponseDto>> GetAllAsync(PersonaFilterDto filter, CancellationToken cancellationToken = default);
}
