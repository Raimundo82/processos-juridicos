#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;
using Processos_Juridicos.Entities;

using Riok.Mapperly.Abstractions;

namespace Processos_Juridicos.DTOs;

public partial class ProcessDto
{
    [Key]
    public int? ProcessId { get; set; }

    [UniqueProcessNiupm]
    [DisplayName("NUIPM")]
    public string Nuipm { get; set; }

    [DisplayName("Tipo de processo")]
    public int? ProcessTypeId { get; set; }

    [DisplayName("Unidade")]
    public int? UnitId { get; set; }

    [DisplayName("Unidade compensadora")]
    public int? CompensatingUnitId { get; set; }

    [DisplayName("Oficial Instrutor")]
    public string OficialInstName { get; set; }

    [DisplayName("Oficial Instrutor Nii")]
    [UserMustBeAllowedToBeOfInst]
    public string OficialInstNii { get; set; }

    [DisplayName("Telefone do Oficial Instrutor")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string OficialInstTelephone { get; set; }

    [DisplayName("Averiguado")]
    public string InvestigatedName { get; set; }

    [DisplayName("Sexo do averiguado")]
    public string InvestigatedGender { get; set; }

    [DisplayName("Data da Ocorrência")]
    public DateOnly? OcurrenceDate { get; set; }

    [DisplayName("Data de Despacho de Nomeação do Instrutor")]
    public DateOnly? DispatchDate { get; set; }

    [DisplayName("Descrição da Ocorrência")]
    [Unicode(false)]
    public string Description { get; set; }

    [DisplayName("Prazo Previsto para encerramento")]
    [FutureDate]
    public DateOnly? DeadlineDate { get; set; }

    [DisplayName("Data de Despacho Final")]
    [FutureDate]
    public DateOnly? FinalDispatchDate { get; set; }

    [DisplayName("Pena aplicada")]
    public int? SentenceId { get; set; }

    [DisplayName("Estado")]
    public required int ProcessStateId { get; set; }

    [DisplayName("Data de criação")]
    public DateOnly? CreatedAt { get; set; }

    [DisplayName("Criado por")]
    public string CreatedByName { get; set; }

    public string CreatedByNii { get; set; }

    [DisplayName("Data de Modificação")]
    public DateOnly? ModifiedAt { get; set; }

    [DisplayName("Modificado por")]
    public string ModifiedByName { get; set; }

    public string ModifiedByNii { get; set; }

    [DisplayName("Acident em serviço")]
    public int? ServiceAccidentId { get; set; }

    [DisplayName("Morto/Ferido")]
    public int? HarmedOrCasualtiesId { get; set; }

    [PositiveValue("Valor Indemnizado a Terceiros")]
    [DisplayName("Valor indeminizado a terceiros")]
    public double? ThirdPartyCompensation { get; set; }

    [PositiveValue("Valor Ressarcido à Marinha")]
    [DisplayName("Valor ressarcido à Marinha")]
    public double? Reimbursement { get; set; }

    [EntityFieldIsRequired("Compensação paga?")]
    [DisplayName("Compensação Paga")]
    public required bool CompensationPaid { get; set; } = false;

    [EntityFieldIsRequired("Comunicado à PJM")]
    [DisplayName("Comunicado à PJM")]
    public required bool ComunicatedToPjm { get; set; } = false;

    [DisplayName("Tipo de crime")]
    public int? CrimeTypeId { get; set; }

    [DisplayName("Segurança militar")]
    public int? MilitarySecurityId { get; set; }

    [DisplayName("Data da comunicação à PJM")]
    public DateOnly? ComunicationDate { get; set; }

    [ForeignKey("UnitId")]
    public virtual UnitDto Unit { get; set; }

    [ForeignKey("HarmedOrCasualtiesId")]
    public virtual HarmedOrCasualtyDto HarmedOrCasualties { get; set; }

    [DisplayName("Artigos infrigidos")]
    [MapperIgnore]
    public List<int?> Infringements { get; set; } = [];

    [MapperIgnore]
    public List<InfringementDto> InfringementDetails { get; set; } = [];

    [ForeignKey("ProcessTypeId")]
    public virtual ProcessTypeDto ProcessType { get; set; }

    [DisplayName("Pena aplicada")]
    [ForeignKey("SentenceId")]
    public virtual SentenceDto Sentence { get; set; }

    [DisplayName("Estado do processo")]
    [ForeignKey("ProcessStateId")]
    public virtual ProcessStateDto ProcessState { get; set; }

    [ForeignKey("ServiceAccidentId")]
    public virtual AccidentTypeDto AccidentType { get; set; }

    [ForeignKey("MilitarySecurityId")]
    public virtual MilitarySecurityDto MilitarySecurity { get; set; }

    [ForeignKey("CrimeTypeId")]
    public virtual CrimeTypeDto CrimeType { get; set; }

    [ForeignKey("CompensatingUnit")]
    public virtual UnitDto CompensatingUnit { get; set; }

    [ForeignKey("OficialInstNii")]
    public virtual UserDto OficialInstrutor { get; set; }

    [ForeignKey("ModifiedByNii")]
    public UserDto ModifiedBy { get; set; }

    [ForeignKey("CreatedByNii")]
    public virtual User CreatedBy { get; set; }

    [MapperIgnore]
    public IFormFile[] ProcessFiles { get; set; }

    [MapperIgnore]
    public List<ProcessFileDto> UploadedFiles { get; set; } = [];

    [NotMapped]
    [MapperIgnore]
    public List<int> FilesToRemove { get; set; } = [];
}
