namespace RecadastramentoApi.DTO;

public sealed class DependenteUpsertDto
{
    public long? FkDependente { get; set; }
    public string? CpfDependente { get; set; }
    public int? FkParentesco { get; set; }
    public int? FkEspecial { get; set; }
    public short? ConsiderarIrrf { get; set; }
    public int? CadastroFkusuario { get; set; }
    public DateTime? CadastroData { get; set; }
    public int? EncerramentoFkusuario { get; set; }
    public string? EncerramentoMotivo { get; set; }
    public string? Observacao { get; set; }
    public bool? ExcluirRegistro { get; set; }
}
