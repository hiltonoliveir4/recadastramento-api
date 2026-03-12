using RecadastramentoApi.DTO;
using RecadastramentoApi.Models;

namespace RecadastramentoApi.Repositories;

public interface IPersonaRepository
{
    Task<Persona?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Persona>> GetAllAsync(PersonaFilterDto filter, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default);

    Task InsertAsync(PersonaUpsertDto dto, CancellationToken cancellationToken = default);

    Task UpdateByCpfAsync(PersonaUpsertDto dto, CancellationToken cancellationToken = default);
}
