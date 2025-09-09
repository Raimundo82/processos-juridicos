using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "O nome de utilizador (NII) é obrigatório")]
    [DisplayName("Utilizador")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "A palavra-passe é obrigatória")]
    [DataType(DataType.Password)]
    [DisplayName("Palavra-passe")]
    public required string Password { get; set; }
}
