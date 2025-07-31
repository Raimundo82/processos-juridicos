#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

using Riok.Mapperly.Abstractions;

namespace Processos_Juridicos.DTOs;

public partial class ProcessDto
{
    [Key]
    public int? ProcessId { get; set; }

    [UniqueProcessNiupm]
    public string Nuipm { get; set; }

    public int? ProcessTypeId { get; set; }

    public int? UnitId { get; set; }

    public int? CompensatingUnitId { get; set; }

    public string OficialInstId { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string OficialInstTelephone { get; set; }

    public string InvestigatedId { get; set; }

    public string InvestigatedGender { get; set; }

    public DateOnly? OcurrenceDate { get; set; }

    public DateOnly? DispatchDate { get; set; }

    //[EntityFieldIsRequired("Descrição")]
    [Unicode(false)]
    public string Description { get; set; }

    [FutureDate]
    public DateOnly? DeadlineDate { get; set; }

    [FutureDate]
    public DateOnly? FinalDispatchDate { get; set; }

    public int? SentenceId { get; set; }

    public required int StateId { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public string CreatedById { get; set; }

    public DateOnly? ModifiedAt { get; set; }

    public string ModifiedById { get; set; }

    public int? ServiceAccidentId { get; set; }

    public int? HarmedOrCasualtiesId { get; set; }

    [PositiveValue("Valor Indemnizado a Terceiros")]
    public double? ThirdPartyCompensation { get; set; }

    [PositiveValue("Valor Indemnizado a Terceiros")]
    public double? Reimbursement { get; set; }

    public int? InfringementId { get; set; }

    [EntityFieldIsRequired("Compensação paga?")]
    public required bool CompensationPaid { get; set; } = false;

    [EntityFieldIsRequired("Comunicado à PJM")]
    public required bool ComunicatedToPjm { get; set; } = false;

    public int? CrimeTypeId { get; set; }

    public int? MilitarySecurityId { get; set; }

    public DateOnly? ComunicationDate { get; set; }

    [ForeignKey("UnitId")]
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

    [NotMapped]
    [MapperIgnore]
    public List<int> FilesToRemove { get; set; } = [];
}
