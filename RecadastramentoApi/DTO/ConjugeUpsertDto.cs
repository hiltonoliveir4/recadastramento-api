namespace RecadastramentoApi.DTO;

public sealed class ConjugeUpsertDto
{
    public long? Id { get; set; }
    public long? FkConjuge { get; set; }
    public string? CpfConjuge { get; set; }
    public int? FkRegimeCasamento { get; set; }
    public DateTime? DataCasamento { get; set; }
    public int? CadastroFkusuario { get; set; }
    public DateTime? CadastroData { get; set; }
    public int? EncerramentoFkusuario { get; set; }
    public DateTime? EncerramentoData { get; set; }
    public string? EncerramentoMotivo { get; set; }
    public string? Observacao { get; set; }
    public bool? ExcluirRegistro { get; set; }
}
