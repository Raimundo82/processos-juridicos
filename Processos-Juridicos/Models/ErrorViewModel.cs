namespace Processos_Juridicos.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public int ErrorCode { get; set; }
    public string? PartialViewName { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
