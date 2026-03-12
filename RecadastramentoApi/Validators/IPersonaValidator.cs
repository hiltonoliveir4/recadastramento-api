using RecadastramentoApi.DTO;

namespace RecadastramentoApi.Validators;

public interface IPersonaValidator
{
    List<string> Validate(PersonaUpsertDto dto);
}
