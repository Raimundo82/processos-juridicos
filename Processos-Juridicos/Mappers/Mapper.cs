using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

using Riok.Mapperly.Abstractions;

namespace Processos_Juridicos.Mappers;

[Mapper]
public static partial class Mapper
{

    // Units Map
    public static partial UnitDto MapToUnitDto(Unit entity);
    public static partial Unit MapToUnit(UnitDto dto);
    public static partial IEnumerable<UnitDto> MapToToUnitDtoEnum(IEnumerable<Unit> entity);

    // Process States Map 

    public static partial ProcessStateDto MapToStateDto(ProcessState entity);
    public static partial ProcessState MapToState(ProcessStateDto dto);
    public static partial IEnumerable<ProcessStateDto> MapToToStateDtoEnum(IEnumerable<ProcessState> entity);

    // Process_Types map

    public static partial ProcessTypeDto MapToProcessTypeDto(ProcessType entity);
    public static partial ProcessType MapToProcessType(ProcessTypeDto dto);
    public static partial IEnumerable<ProcessTypeDto> MapToToProcessTypeDtoEnum(IEnumerable<ProcessType> entity);

    // Sentences map
    public static partial SentenceDto MapToSentenceDto(Sentence entity);
    public static partial Sentence MapToSentence(SentenceDto dto);
    public static partial IEnumerable<SentenceDto> MapToToSentenceDtoEnum(IEnumerable<Sentence> entity);

    // Harmed or Casualties map

    public static partial HarmedOrCasualtyDto MapToHarmedOrCasualtiesDto(HarmedOrCasualty entity);
    public static partial HarmedOrCasualty MapToHarmedOrCasualties(HarmedOrCasualtyDto dto);
    public static partial IEnumerable<HarmedOrCasualtyDto> MapToToHarmedOrCasualtiesEnum(IEnumerable<HarmedOrCasualty> entity);

    // Infringements map

    public static partial InfringementDto MapToInfringementsDto(Infringement entity);
    public static partial Infringement MapToInfringements(InfringementDto dto);
    public static partial IEnumerable<InfringementDto> MapToToInfringementsEnum(IEnumerable<Infringement> entity);

    // Files map

    public static partial ProcessFileDto MapToFilesDto(ProcessFile entity);
    public static partial ProcessFile MapToFiles(ProcessFileDto dto);
    public static partial IEnumerable<ProcessFileDto> MapToToFilesEnum(IEnumerable<ProcessFile> entity);

    // Processes map


    public static partial ProcessDto MapToProcessesDto(Process entity);
    public static partial Process MapToProcesses(ProcessDto dto);
    public static partial IEnumerable<ProcessDto> MapToToProcessesEnum(IEnumerable<Process> entity);


    // Accident Types map

    public static partial AccidentTypeDto MapToAccidenTypeDto(AccidentType entity);
    public static partial AccidentType MapToAccidentType(AccidentTypeDto dto);
    public static partial IEnumerable<AccidentTypeDto> MapToAccidentTypeEnum(IEnumerable<AccidentType> entity);

    // Crime Types map

    public static partial CrimeTypeDto MapToCrimeTypeDto(CrimeType entity);
    public static partial CrimeType MapToCrimeType(CrimeTypeDto dto);
    public static partial IEnumerable<CrimeTypeDto> MapToCrimeTypeEnum(IEnumerable<CrimeType> entity);

    //Military Security map

    public static partial MilitarySecurityDto MapToMilitarySecurityDto(MilitarySecurity entity);
    public static partial MilitarySecurity MapToMilitarySecurity(MilitarySecurityDto dto);
    public static partial IEnumerable<MilitarySecurityDto> MapToMilitarySecurityEnum(IEnumerable<MilitarySecurity> entity);

    //Process state transition map

    public static partial StateTransitionDto MapToStateTransitionDto(StateTransition entity);
    public static partial StateTransition MapToStateTransition(StateTransitionDto dto);
    public static partial IEnumerable<StateTransitionDto> MapToStateTransitionEnum(IEnumerable<StateTransition> entity);

}
