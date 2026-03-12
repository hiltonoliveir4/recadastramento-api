using RecadastramentoApi.DTO;

namespace RecadastramentoApi.Validators;

public sealed class PersonaUpsertValidator : IPersonaValidator
{
    public List<string> Validate(PersonaUpsertDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Cpf))
        {
            errors.Add("cpf is required.");
        }
        else if (dto.Cpf.Length != 11 || !dto.Cpf.All(char.IsDigit))
        {
            errors.Add("cpf must contain exactly 11 numeric characters.");
        }

        if (dto.SexoEnum <= 0)
        {
            errors.Add("sexo_enum is required.");
        }

        if (dto.EtniaEnum <= 0)
        {
            errors.Add("etnia_enum is required.");
        }

        if (dto.NacionalidadeEnum <= 0)
        {
            errors.Add("nacionalidade_enum is required.");
        }

        if (dto.NacionalidadePais <= 0)
        {
            errors.Add("nacionalidade_pais is required.");
        }

        return errors;
    }
}
