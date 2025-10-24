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
    [ExcludedFromValidation]
    public string Nuipm { get; set; }

    [DisplayName("Tipo de processo")]
    public int? ProcessTypeId { get; set; }

    [DisplayName("Unidade")]
    public int? UnitId { get; set; }

    [DisplayName("Unidade compensadora")]
    [ExcludedFromValidation]
    public int? CompensatingUnitId { get; set; }

    [DisplayName("Oficial Instrutor")]
    public string OficialInstName { get; set; }

    [DisplayName("Oficial Instrutor Nii")]
    [UserMustBeAllowedToBeOfInst]
    [ExcludedFromValidation]
    public string OficialInstNii { get; set; }

    [DisplayName("Telefone do Oficial Instrutor")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string OficialInstTelephone { get; set; }

    [DisplayName("Averiguado")]
    [ExcludedFromValidation]
    public string InvestigatedName { get; set; }

    [DisplayName("Sexo do averiguado")]
    public string InvestigatedGender { get; set; }

    [DisplayName("Data da Ocorrência")]
    public DateOnly? OcurrenceDate { get; set; }

    [DisplayName("Data de Despacho de Nomeação do Instrutor")]
    [ExcludedFromValidation]
    public DateOnly? DispatchDate { get; set; }

    [DisplayName("Descrição da Ocorrência")]
    [ExcludedFromValidation]
    [Unicode(false)]
    public string Description { get; set; }

    [DisplayName("Prazo Previsto para encerramento")]
    [ExcludedFromValidation]
    [FutureDate]
    public DateOnly? DeadlineDate { get; set; }

    [DisplayName("Data de Despacho Final")]
    [ExcludedFromValidation]
    [FutureDate]
    public DateOnly? FinalDispatchDate { get; set; }

    [DisplayName("Pena aplicada")]
    [ExcludedFromValidation]
    public int? SentenceId { get; set; }

    [DisplayName("Estado")]
    public required int ProcessStateId { get; set; }

    [DisplayName("Data de criação")]
    public DateOnly? CreatedAt { get; set; }

    [DisplayName("Criado por")]
    [ExcludedFromValidation]
    public string CreatedByName { get; set; }

    [ExcludedFromValidation]
    public string CreatedByNii { get; set; }

    [DisplayName("Data de Modificação")]
    [ExcludedFromValidation]
    public DateOnly? ModifiedAt { get; set; }

    [DisplayName("Modificado por")]
    [ExcludedFromValidation]
    public string ModifiedByName { get; set; }

    [ExcludedFromValidation]
    public string ModifiedByNii { get; set; }

    [DisplayName("Acident em serviço")]
    [ExcludedFromValidation]
    public int? ServiceAccidentId { get; set; }

    [DisplayName("Morto/Ferido")]
    [ExcludedFromValidation]
    public int? HarmedOrCasualtiesId { get; set; }

    [PositiveValue("Valor Indemnizado a Terceiros")]
    [DisplayName("Valor indeminizado a terceiros")]
    [ExcludedFromValidation]
    public double? ThirdPartyCompensation { get; set; }

    [PositiveValue("Valor Ressarcido à Marinha")]
    [DisplayName("Valor ressarcido à Marinha")]
    [ExcludedFromValidation]
    public double? Reimbursement { get; set; }

    [EntityFieldIsRequired("Compensação paga?")]
    [DisplayName("Compensação Paga")]
    [ExcludedFromValidation]
    public required bool CompensationPaid { get; set; } = false;

    [EntityFieldIsRequired("Comunicado à PJM")]
    [DisplayName("Comunicado à PJM")]
    [ExcludedFromValidation]
    public required bool ComunicatedToPjm { get; set; } = false;

    [DisplayName("Tipo de crime")]
    [ExcludedFromValidation]
    public int? CrimeTypeId { get; set; }

    [DisplayName("Segurança militar")]
    [ExcludedFromValidation]
    public int? MilitarySecurityId { get; set; }

    [DisplayName("Data da comunicação à PJM")]
    [ExcludedFromValidation]
    public DateOnly? ComunicationDate { get; set; }

    [ForeignKey("UnitId")]
    [ExcludedFromValidation]
    public virtual UnitDto Unit { get; set; }

    [ForeignKey("HarmedOrCasualtiesId")]
    [ExcludedFromValidation]
    public virtual HarmedOrCasualtyDto HarmedOrCasualties { get; set; }

    [DisplayName("Artigos infrigidos")]
    [MapperIgnore]
    public List<int?> Infringements { get; set; } = [];

    [MapperIgnore]
    public List<InfringementDto> InfringementDetails { get; set; } = [];

    [ForeignKey("ProcessTypeId")]
    [ExcludedFromValidation]
    public virtual ProcessTypeDto ProcessType { get; set; }

    [DisplayName("Pena aplicada")]
    [ForeignKey("SentenceId")]
    [ExcludedFromValidation]
    public virtual SentenceDto Sentence { get; set; }

    [DisplayName("Estado do processo")]
    [ExcludedFromValidation]
    [ForeignKey("ProcessStateId")]
    public virtual ProcessStateDto ProcessState { get; set; }

    [ForeignKey("ServiceAccidentId")]
    [ExcludedFromValidation]
    public virtual AccidentTypeDto AccidentType { get; set; }

    [ExcludedFromValidation]
    [ForeignKey("MilitarySecurityId")]
    public virtual MilitarySecurityDto MilitarySecurity { get; set; }

    [ForeignKey("CrimeTypeId")]
    [ExcludedFromValidation]
    public virtual CrimeTypeDto CrimeType { get; set; }

    [ExcludedFromValidation]
    [ForeignKey("CompensatingUnit")]
    public virtual UnitDto CompensatingUnit { get; set; }

    [ForeignKey("OficialInstNii")]
    [ExcludedFromValidation]
    public virtual UserDto OficialInstrutor { get; set; }

    [ExcludedFromValidation]
    [ForeignKey("ModifiedByNii")]
    public UserDto ModifiedBy { get; set; }

    [ForeignKey("CreatedByNii")]
    [ExcludedFromValidation]
    public virtual User CreatedBy { get; set; }

    [ExcludedFromValidation]
    [MapperIgnore]
    public IFormFile[] ProcessFiles { get; set; }

    [ExcludedFromValidation]
    [MapperIgnore]
    public List<ProcessFileDto> UploadedFiles { get; set; } = [];

    [NotMapped]
    [ExcludedFromValidation]
    [MapperIgnore]
    public List<int> FilesToRemove { get; set; } = [];
}
