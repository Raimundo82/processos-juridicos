using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Riok.Mapperly.Abstractions;

namespace Processos_Juridicos.Mappers;

[Mapper]
public static partial class Mapper
{

    // Units Map
    public static partial UnitsDTO MapToUnitDto(Units entity);
    public static partial Units MapToUnit(UnitsDTO dto);
    public static partial IEnumerable<UnitsDTO> MapToToUnitDtoEnum(IEnumerable<Units> entity);

    // States Map 

    public static partial StatesDTO MapToStateDto(States entity);
    public static partial States MapToState (StatesDTO dto);
    public static partial IEnumerable<StatesDTO> MapToToStateDtoEnum(IEnumerable<States> entity);

    // Process_Types map

    public static partial Process_typesDTO MapToStateTypeDto(Process_types entity);
    public static partial Process_types MapToProcessType(Process_typesDTO dto);
    public static partial IEnumerable<Process_typesDTO> MapToToProcessTypeDtoEnum(IEnumerable<Process_types> entity);

    // Sentences map
    public static partial SentencesDTO MapToSentenceDto(Sentences entity);
    public static partial Sentences MapToSentence(SentencesDTO dto);
    public static partial IEnumerable<SentencesDTO> MapToToSentenceDtoEnum(IEnumerable<Sentences> entity);

    // Harmed or Casualties map

    public static partial Harmed_or_casualtiesDTO MapToHarmedOrCasualtiesDto(Harmed_or_casualties entity);
    public static partial Harmed_or_casualties MapToHarmedOrCasualties(Harmed_or_casualtiesDTO dto);
    public static partial IEnumerable<Harmed_or_casualtiesDTO> MapToToHarmedOrCasualtiesEnum(IEnumerable<Harmed_or_casualties> entity);

    // Infringements map

    public static partial InfringementsDTO MapToInfringementsDto(Infringements entity);
    public static partial Infringements MapToInfringements(InfringementsDTO dto);
    public static partial IEnumerable<InfringementsDTO> MapToToInfringementsEnum(IEnumerable<Infringements> entity);

    // Sectors map

    public static partial SectorsDTO MapToSectorsDto(Infringements entity);
    public static partial Sectors MapToSectors(InfringementsDTO dto);
    public static partial IEnumerable<SectorsDTO> MapToToSectorsEnum(IEnumerable<Sectors> entity);


    // Files map

    public static partial FilesDTO MapToFilesDto(Files entity);
    public static partial Files MapToFiles(FilesDTO dto);
    public static partial IEnumerable<FilesDTO> MapToToFilesEnum(IEnumerable<Files> entity);

    // Processes map

    public static partial ProcessesDTO MapToProcessesDto(Processes entity);
    public static partial Processes MapToProcesses(ProcessesDTO dto);
    public static partial IEnumerable<ProcessesDTO> MapToToProcessesEnum(IEnumerable<Processes> entity);

}
