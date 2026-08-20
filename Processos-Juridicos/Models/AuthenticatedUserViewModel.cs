namespace Processos_Juridicos.Models;

public class AuthenticatedUserViewModel
{
    public bool IsAuthenticated { get; set; } = false;
    public string DisplayName { get; set; } = "Utilizador";
    public string Nii { get; set; } = string.Empty;
    public string Photo { get; set; } = "/images/default-avatar.webp";
}
