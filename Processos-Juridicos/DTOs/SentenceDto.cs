#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

public partial class SentenceDto
{
    [Key]
    [Required]
    public int SentenceId { get; set; }

    [DisplayName("Nome da Sentença")]
    [Required(ErrorMessage = "O Nome da Sentença é obrigatório")]
    [StringLength(50)]
    [Unicode(false)]
    public string SentenceName { get; set; }
}