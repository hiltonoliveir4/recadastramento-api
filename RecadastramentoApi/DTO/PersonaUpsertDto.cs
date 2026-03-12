using RecadastramentoApi.Models;

namespace RecadastramentoApi.DTO;

public sealed class PersonaUpsertDto : PersonaFieldValues
{
    public long? Id { get; set; }
}
