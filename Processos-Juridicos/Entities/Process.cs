#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class Process
{
    [Key]
    [Column("process_id")]
    public int? ProcessId { get; set; }

    [Column("nuipm")]
    public string Nuipm { get; set; }

    [Column("process_type_id")]
    public int? ProcessTypeId { get; set; }

    [Column("unit_id")]
    public int? UnitId { get; set; }

    [Column("oficial_inst_telephone")]
    public string OficialInstTelephone { get; set; }

    [Column("oficial_inst_name")]
    public string OficialInstName { get; set; }

    [Column("compensating_unit_id")]
    public int? CompensatingUnitId { get; set; }

    [Column("investigated_name")]
    public string InvestigatedName { get; set; }

    [Column("investigated_gender")]
    public string InvestigatedGender { get; set; }

    [Column("occurrence_date")]
    public DateOnly? OcurrenceDate { get; set; }

    [Column("dispatch_date")]
    public DateOnly? DispatchDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Column("description")]
    public string Description { get; set; }

    [Column("deadline_date")]
    public DateOnly? DeadlineDate { get; set; }

    [Column("final_dispatch_date")]
    public DateOnly? FinalDispatchDate { get; set; }

    [Column("sentence_id")]
    public int? SentenceId { get; set; }

    [Column("state_id")]
    public int ProcessStateId { get; set; }

    [Column("created_at")]
    public DateOnly? CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Column("created_by")]
    public string CreatedBy { get; set; }

    [Column("modified_at")]
    public DateOnly? ModifiedAt { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    [Column("modified_by")]
    public string ModifiedBy { get; set; }

    [Column("service_accident_id")]
    public int? ServiceAccidentId { get; set; }

    [Column("harmed_or_casualties_id")]
    public int? HarmedOrCasualtiesId { get; set; }

    [Column("third_party_compensation")]
    public double? ThirdPartyCompensation { get; set; }

    [Column("reimbursement")]
    public double? Reimbursement { get; set; }

    [Column("crime_type_id")]
    public int? CrimeTypeId { get; set; }

    [Column("compensation_paid_by_unit")]
    public bool CompensationPaid { get; set; } = false;

    [Column("comunicated_pjm")]
    public bool ComunicatedToPjm { get; set; } = false;

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

    public virtual ICollection<Infringement> Infringements { get; set; } = [];

    [ForeignKey("ProcessTypeId")]
    public virtual ProcessType ProcessType { get; set; }

    [ForeignKey("SentenceId")]
    public virtual Sentence Sentence { get; set; }

    [ForeignKey("ProcessStateId")]
    public virtual ProcessState ProcessState { get; set; }

    [ForeignKey("ServiceAccidentId")]
    public virtual AccidentType AccidentType { get; set; }

    [ForeignKey("MilitarySecurityId")]
    public virtual MilitarySecurity MilitarySecurity { get; set; }

    [ForeignKey("CrimeTypeId")]
    public virtual CrimeType CrimeType { get; set; }


}
