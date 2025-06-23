#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class Process
{
    [Key]
    [Column("process_id")]
    public int ProcessId { get; set; }

    [Column("nuipm")]
    public string Nuipm { get; set; }

    [Column("process_type_id")]
    public int? ProcessTypeId { get; set; }

    [Column("unit_id")]
    public int? UnitId { get; set; }

    [Column("oficial_inst_telephone")]
    public string OficialInstTelephone { get; set; }

    [Column("oficial_inst_id")]
    public int? OficialInstId { get; set; }

    [Column("compensating_unit_id")]
    public int? CompensatingUnitId { get; set; }

    [Column("investigated_id")]
    public int? InvestigatedId { get; set; }

    [Column("investigated_gender")]
    public string InvestigatedGender { get; set; }

    [Column("occurrence_date")]
    public DateOnly? OcurrenceDate { get; set; }

    [Column("dispatch_date")]
    public DateOnly? DispatchDate { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("deadline_date")]
    public DateOnly? DeadlineDate { get; set; }

    [Column("final_dispatch_date")]
    public DateOnly? FinalDispatchDate { get; set; }

    [Column("sentence_id")]
    public int? SentenceId { get; set; }

    [Column("state_id")]
    public int StateId { get; set; }

    [Column("created_at")]
    public DateOnly? CreatedAt { get; set; }

    [Column("created_by_id")]
    public int? CreatedById { get; set; }

    [Column("modified_at")]
    public DateOnly? ModifiedAt { get; set; }

    [Column("modified_by_id")]
    public int? ModifiedById { get; set; }

    [Column("service_accident_id")]
    public int? ServiceAccidentId { get; set; }

    [Column("harmed_or_casualties_id")]
    public int? HarmedOrCasualtiesId { get; set; }

    [Column("third_party_compensation")]
    public double? ThirdPartyCompensation { get; set; }

    [Column("reimbursement")]
    public double? Reimbursement { get; set; }

    [Column("infringement_id")]
    public int? InfringementId { get; set; }

    [Column("crime_type_id")]
    public int? CrimeTypeId { get; set; }

    [Column("compensation_paid_by_unit")]
    [Required]
    public bool CompensationPaid { get; set; }

    [Column("comunicated_pjm")]
    [Required]
    public bool ComunicatedToPjm { get; set; }

    [Column("pjm_comunication_date")]
    public DateOnly? ComunicationDate { get; set; }

    [Column("military_security_id")]
    public int? MilitarySecurityId { get; set; }

    [ForeignKey("UnitId")]
    public virtual Unit Unit { get; set; }

    [ForeignKey("CompensatingUnitId")]
    public virtual Unit CompensatingUnit { get; set; }

    [ForeignKey("HarmedOrCasualtiesId")]
    public virtual HarmedOrCasualty HarmedOrCasualties { get; set; }

    [ForeignKey("InfringementId")]
    public virtual Infringement Infringement { get; set; }

    [ForeignKey("ProcessTypeId")]
    public virtual ProcessType ProcessType { get; set; }

    [ForeignKey("SentenceId")]
    public virtual Sentence Sentence { get; set; }

    [ForeignKey("StateId")]
    public virtual State State { get; set; }

    [ForeignKey("ServiceAccidentId")]
    public virtual AccidentType AccidentType { get; set; }

    [ForeignKey("MilitarySecurityId")]
    public virtual MilitarySecurity MilitarySecurity { get; set; }

    [ForeignKey("CrimeTypeId")]
    public virtual CrimeType CrimeType { get; set; }


}