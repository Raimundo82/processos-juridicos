#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

[Table("Process_files")]
public partial class ProcessFile
{
    [Key]
    [Column("process_file_id")]
    public int? ProcessFileId { get; set; }

    [Column("process_file_name")]
    public string ProcessFileName { get; set; }

    [Column("process_file_type")]
    public string ProcessFileType { get; set; }

    [Column("process_file_content")]
    public byte[] ProcessFileContent { get; set; }

    [Column("process_id")]
    public int ProcessId { get; set; }

    [Column("row_guid")]
    public int RowGuid { get; set; }

    [ForeignKey("ProcessId")]
    public virtual Process Process { get; set; }
}
