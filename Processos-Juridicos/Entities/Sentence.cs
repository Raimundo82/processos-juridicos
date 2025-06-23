#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class Sentence
{
    [Key]
    [Column("sentence_id")]
    public int SentenceId { get; set; }

    [Column("sentence_name")]
    public string SentenceName { get; set; }

}