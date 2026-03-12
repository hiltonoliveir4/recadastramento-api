using RecadastramentoApi.DTO;
using RecadastramentoApi.Models;
using RecadastramentoApi.Repositories;
using RecadastramentoApi.Validators;

namespace RecadastramentoApi.Services;

public sealed class PersonaService(IPersonaRepository personaRepository, IPersonaValidator validator) : IPersonaService
{
    public async Task<PersonaBatchUpsertResponseDto> UpsertBatchAsync(IReadOnlyList<PersonaUpsertDto> personas, CancellationToken cancellationToken = default)
    {
        var response = new PersonaBatchUpsertResponseDto();

        for (var index = 0; index < personas.Count; index++)
        {
            var dto = personas[index];
            dto.Cpf = dto.Cpf.Trim();

            var validationErrors = validator.Validate(dto);
            if (validationErrors.Count > 0)
            {
                response.Errors.Add($"index {index} | cpf {dto.Cpf}: {string.Join("; ", validationErrors)}");
                continue;
            }

            try
            {
                var exists = await personaRepository.ExistsByCpfAsync(dto.Cpf, cancellationToken);
                if (exists)
                {
                    await personaRepository.UpdateByCpfAsync(dto, cancellationToken);
                    response.Updated++;
                }
                else
                {
                    await personaRepository.InsertAsync(dto, cancellationToken);
                    response.Inserted++;
                }
            }
            catch (Exception exception)
            {
                response.Errors.Add($"index {index} | cpf {dto.Cpf}: {exception.Message}");
            }
        }

        return response;
    }

    public async Task<PersonaResponseDto?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var persona = await personaRepository.GetByCpfAsync(cpf.Trim(), cancellationToken);
        return persona?.ToResponseDto();
    }

    public async Task<IReadOnlyList<PersonaResponseDto>> GetAllAsync(PersonaFilterDto filter, CancellationToken cancellationToken = default)
    {
        var personas = await personaRepository.GetAllAsync(filter, cancellationToken);
        return personas.Select(persona => persona.ToResponseDto()).ToList();
    }
}
