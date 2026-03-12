namespace RecadastramentoApi.DTO;

public sealed class PersonaBatchUpsertResponseDto
{
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public List<string> Errors { get; set; } = [];
}
