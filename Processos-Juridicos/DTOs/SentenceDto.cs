#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class SentenceDto
{
    [Key]
    public int? SentenceId { get; set; }

    [DisplayName("Nome da Sentença")]
    [EntityFieldIsRequired("Nome da Sentença")]
    [Attributes.MaxLength(50, "Nome da Sentença")]
    [Unicode(false)]
    [UniqueSentenceName]
    public string SentenceName { get; set; }
}
