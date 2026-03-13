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
        var successfulPersonas = new List<(int Index, PersonaUpsertDto Dto)>();

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

                successfulPersonas.Add((index, dto));
            }
            catch (Exception exception)
            {
                response.Errors.Add($"index {index} | cpf {dto.Cpf}: {exception.Message}");
            }
        }

        foreach (var item in successfulPersonas)
        {
            try
            {
                var personaId = await personaRepository.GetIdByCpfAsync(item.Dto.Cpf, cancellationToken);
                if (!personaId.HasValue)
                {
                    throw new InvalidOperationException($"Persona id not found for cpf {item.Dto.Cpf}.");
                }

                if (item.Dto.Dependentes.Count > 0)
                {
                    await personaRepository.UpsertDependentesAsync(personaId.Value, item.Dto.Dependentes, cancellationToken);
                }

                if (item.Dto.Manutencoes.Count > 0)
                {
                    await personaRepository.UpsertManutencoesAsync(personaId.Value, item.Dto.Manutencoes, cancellationToken);
                }

                if (item.Dto.Anexos.Count > 0)
                {
                    await personaRepository.UpsertAnexosAsync(personaId.Value, item.Dto.Anexos, cancellationToken);
                }

                if (item.Dto.Conjuges.Count > 0)
                {
                    await personaRepository.UpsertConjugesAsync(personaId.Value, item.Dto.Conjuges, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                response.Errors.Add($"index {item.Index} | cpf {item.Dto.Cpf} | relacionamentos: {exception.Message}");
            }
        }

        return response;
    }

    public async Task<PersonaResponseDto?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        var persona = await personaRepository.GetByCpfAsync(cpf.Trim(), cancellationToken);
        if (persona is null)
        {
            return null;
        }

        var response = persona.ToResponseDto();
        await EnrichResponseAsync([response], cancellationToken);
        return response;
    }

    public async Task<IReadOnlyList<PersonaResponseDto>> GetAllAsync(PersonaFilterDto filter, CancellationToken cancellationToken = default)
    {
        var personas = await personaRepository.GetAllAsync(filter, cancellationToken);
        var responses = personas.Select(persona => persona.ToResponseDto()).ToList();
        await EnrichResponseAsync(responses, cancellationToken);
        return responses;
    }

    private async Task EnrichResponseAsync(List<PersonaResponseDto> personas, CancellationToken cancellationToken)
    {
        if (personas.Count == 0)
        {
            return;
        }

        var ids = personas.Select(item => item.Id).ToArray();

        var dependentesTask = personaRepository.GetDependentesByResponsavelIdsAsync(ids, cancellationToken);
        var conjugesTask = personaRepository.GetConjugesByPersonaIdsAsync(ids, cancellationToken);
        var anexosTask = personaRepository.GetAnexosByPersonaIdsAsync(ids, cancellationToken);
        var manutencoesTask = personaRepository.GetManutencoesByPersonaIdsAsync(ids, cancellationToken);

        await Task.WhenAll(dependentesTask, conjugesTask, anexosTask, manutencoesTask);

        var dependentesLookup = dependentesTask.Result;
        var conjugesLookup = conjugesTask.Result;
        var anexosLookup = anexosTask.Result;
        var manutencoesLookup = manutencoesTask.Result;

        foreach (var persona in personas)
        {
            persona.Dependentes = dependentesLookup.TryGetValue(persona.Id, out var dependentes) ? dependentes : [];
            persona.Conjuges = conjugesLookup.TryGetValue(persona.Id, out var conjuges) ? conjuges : [];
            persona.Anexos = anexosLookup.TryGetValue(persona.Id, out var anexos) ? anexos : [];
            persona.Manutencoes = manutencoesLookup.TryGetValue(persona.Id, out var manutencoes) ? manutencoes : [];
        }
    }
}
