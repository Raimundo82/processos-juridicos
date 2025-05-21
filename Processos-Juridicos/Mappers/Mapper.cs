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


}
