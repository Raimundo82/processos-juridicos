#nullable disable
using Microsoft.EntityFrameworkCore;
using Riok.Mapperly.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.DTOs;

public partial class ProcessDto
{
    [Key]
    public int ProcessId { get; set; }

    public string Nuipm { get; set; }

    public int? ProcessTypeId { get; set; }

    public int? UnitId { get; set; }
    
    public int? CompensatingUnitId { get; set; }

    public int? OficialInstId { get; set; }

    public string OficialInstTelephone { get; set; }

    public int? InvestigatedId { get; set; }

    public string InvestigatedGender { get; set; }

    public DateOnly? OcurrenceDate { get; set; }

    public DateOnly? DispatchDate { get; set; }

    [Required]
    [Unicode(false)]
    public string Description { get; set; }

    public DateOnly? DeadlineDate { get; set; }

    public DateOnly? FinalDispatchDate { get; set; }

    public int? SentenceId { get; set; }

    public int StateId { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public int? CreatedById { get; set; }

    public DateOnly? ModifiedAt { get; set; }

    public int? ModifiedById { get; set; }

    public int? ServiceAccidentId { get; set; }

    public int? HarmedOrCasualtiesId { get; set; }

    public double? ThirdPartyCompensation { get; set; }

    public double? Reimbursement { get; set; }

    public int? InfringementId { get; set; }
    [Required]
    public bool? CompensationPaid { get; set; }
    [Required]
    public bool? ComunicatedToPjm { get; set; }

    public int? CrimeTypeId { get; set; }

    public int? MilitarySecurityId { get; set; }

    public DateOnly? ComunicationDate { get; set; }

    [ForeignKey("UnitId")]
    [Required]
    public virtual UnitDto Unit { get; set; }

    [ForeignKey("HarmedOrCasualtiesId")]
    public virtual HarmedOrCasualtyDto HarmedOrCasualties { get; set; }

    [ForeignKey("InfringementId")]
    public virtual InfringementDto Infringement { get; set; }

    [ForeignKey("ProcessTypeId")]
    public virtual ProcessTypeDto ProcessType { get; set; }

    [ForeignKey("SentenceId")]
    public virtual SentenceDto Sentence { get; set; }

    [ForeignKey("StateId")]
    public virtual StateDto State { get; set; }
    
    [ForeignKey("ServiceAccidentId")]
    public virtual AccidentTypeDto AccidentType { get; set; }

    [ForeignKey("MilitarySecurityId")]
    public virtual MilitarySecurityDto MilitarySecurity { get; set; }

    [ForeignKey("CrimeTypeId")]
    public virtual CrimeTypeDto CrimeType { get; set; }

    [ForeignKey("CompensatingUnit")]
    public virtual UnitDto CompensatingUnit { get; set; }

    [MapperIgnore]
    public IFormFile[] ProcessFiles { get; set; }

    [MapperIgnore]
    public List<ProcessFileDto> UploadedFiles { get; set; } = [];
}