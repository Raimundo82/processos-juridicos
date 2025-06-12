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
    public int? Nuipm { get; set; }

    [Column("process_type_id")]
    public int? ProcessTypeId { get; set; }

    [Column("unit_id")]
    public int? UnitId { get; set; }

    [Column("oficial_inst_id")]
    public int? OficialInstId { get; set; }

    [Column("investigated_id")]
    public int? InvestigatedId { get; set; }

    [Column("investigated_gender_id")]
    public int? InvestigatedGenderId { get; set; }

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


    [ForeignKey("HarmedOrCasualtiesId")]
    [InverseProperty("Processes")]
    public HarmedOrCasualty HarmedOrCasualties { get; set; }

    [ForeignKey("InfringementId")]
    [InverseProperty("Processes")]
    public Infringement Infringement { get; set; }

    [ForeignKey("ProcessTypeId")]
    [InverseProperty("Processes")]
    public ProcessType ProcessType { get; set; }

    [ForeignKey("SentenceId")]
    [InverseProperty("Processes")]
    public Sentence Sentence { get; set; }

    [ForeignKey("AccidentTypeId")]
    [InverseProperty("Processes")]
    public AccidentType AccidentType { get; set; }

    [ForeignKey("StateId")]
    [InverseProperty("Processes")]
    [Column("state")]
    public State State { get; set; }

    [ForeignKey("UnitId")]
    [InverseProperty("Processes")]
    [Column("unit_id")]
    public Unit Unit { get; set; }
}