namespace Processos_Juridicos.DTOs;

public class FileSaveResult
{
    public bool IsSuccess { get; set; }
    public int SavedFileId { get; set; }
    public string? ErrorMessage { get; set; }
}
