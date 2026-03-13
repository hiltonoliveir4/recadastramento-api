using RecadastramentoApi.Models;

namespace RecadastramentoApi.DTO;

public sealed class PersonaResponseDto : PersonaFieldValues
{
    public long Id { get; set; }

    public List<DependenteUpsertDto> Dependentes { get; set; } = [];

    public List<ConjugeUpsertDto> Conjuges { get; set; } = [];

    public List<AnexoUpsertDto> Anexos { get; set; } = [];

    public List<ManutencaoUpsertDto> Manutencoes { get; set; } = [];
}
