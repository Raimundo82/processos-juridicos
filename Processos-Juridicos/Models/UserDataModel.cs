namespace Processos_Juridicos.Models;

public class UserDataModel
{
    public string? DisplayName { get; set; }
    public string? UserName { get; set; }
    public string? FullUser { get; set; }
    public string? Nii { get; set; }
    public string? Unit { get; set; }
    public string? PhotoBase64 { get; set; }
    public List<string>? Groups { get; set; }
    public string? Email { get; set; }
}
