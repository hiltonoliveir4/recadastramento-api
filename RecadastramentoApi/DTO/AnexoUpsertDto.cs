namespace RecadastramentoApi.DTO;

public sealed class AnexoUpsertDto
{
    public long? Id { get; set; }
    public int? FkAnexoTipo { get; set; }
    public int? Obrigatorio { get; set; }
    public DateTime? EmissaoData { get; set; }
    public DateTime? ValidadeData { get; set; }
    public int? CadastroFkusuario { get; set; }
    public DateTime? CadastroData { get; set; }
    public string? Url { get; set; }
    public byte[]? Arquivo { get; set; }
    public string? Observacao { get; set; }
}
