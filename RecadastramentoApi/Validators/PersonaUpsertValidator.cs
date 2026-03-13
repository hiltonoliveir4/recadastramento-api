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

        for (var index = 0; index < dto.Dependentes.Count; index++)
        {
            var dependente = dto.Dependentes[index];

            if ((!dependente.FkDependente.HasValue || dependente.FkDependente <= 0) &&
                string.IsNullOrWhiteSpace(dependente.CpfDependente))
            {
                errors.Add($"dependentes[{index}]: fk_dependente or cpf_dependente is required.");
            }

            if (dependente.FkDependente.HasValue && dependente.FkDependente <= 0)
            {
                errors.Add($"dependentes[{index}]: fk_dependente must be greater than zero.");
            }

            if (!string.IsNullOrWhiteSpace(dependente.CpfDependente) && !IsValidCpf(dependente.CpfDependente))
            {
                errors.Add($"dependentes[{index}]: cpf_dependente must contain exactly 11 numeric characters.");
            }

            if (!dependente.FkParentesco.HasValue || dependente.FkParentesco <= 0)
            {
                errors.Add($"dependentes[{index}]: fk_parentesco is required.");
            }

            if (!dependente.CadastroFkusuario.HasValue || dependente.CadastroFkusuario <= 0)
            {
                errors.Add($"dependentes[{index}]: cadastro_fkusuario is required.");
            }
        }

        for (var index = 0; index < dto.Anexos.Count; index++)
        {
            var anexo = dto.Anexos[index];
            if (!anexo.FkAnexoTipo.HasValue || anexo.FkAnexoTipo <= 0)
            {
                errors.Add($"anexos[{index}]: fk_anexo_tipo is required.");
            }
        }

        for (var index = 0; index < dto.Conjuges.Count; index++)
        {
            var conjuge = dto.Conjuges[index];

            if ((!conjuge.FkConjuge.HasValue || conjuge.FkConjuge <= 0) &&
                string.IsNullOrWhiteSpace(conjuge.CpfConjuge))
            {
                errors.Add($"conjuges[{index}]: fk_conjuge or cpf_conjuge is required.");
            }

            if (conjuge.FkConjuge.HasValue && conjuge.FkConjuge <= 0)
            {
                errors.Add($"conjuges[{index}]: fk_conjuge must be greater than zero.");
            }

            if (!string.IsNullOrWhiteSpace(conjuge.CpfConjuge) && !IsValidCpf(conjuge.CpfConjuge))
            {
                errors.Add($"conjuges[{index}]: cpf_conjuge must contain exactly 11 numeric characters.");
            }

            if (!conjuge.FkRegimeCasamento.HasValue || conjuge.FkRegimeCasamento <= 0)
            {
                errors.Add($"conjuges[{index}]: fk_regime_casamento is required.");
            }

            if (!conjuge.DataCasamento.HasValue)
            {
                errors.Add($"conjuges[{index}]: data_casamento is required.");
            }

            if (!conjuge.CadastroFkusuario.HasValue || conjuge.CadastroFkusuario <= 0)
            {
                errors.Add($"conjuges[{index}]: cadastro_fkusuario is required.");
            }
        }

        return errors;
    }

    private static bool IsValidCpf(string cpf)
    {
        var trimmed = cpf.Trim();
        return trimmed.Length == 11 && trimmed.All(char.IsDigit);
    }
}
