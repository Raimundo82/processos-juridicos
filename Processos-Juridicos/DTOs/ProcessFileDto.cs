#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Processos_Juridicos.Entities;

namespace Processos_Juridicos.DTOs;

public partial class ProcessFileDto
{
    [Key]
    public int? ProcessFileId { get; set; }

    public string ProcessFileName { get; set; }

    public string ProcessFileType { get; set; }

    public byte[] ProcessFileContent { get; set; }

    public int ProcessId { get; set; }

    public int RowGuid { get; set; }

    [ForeignKey("ProcessId")]
    public virtual Process Process { get; set; }
}
