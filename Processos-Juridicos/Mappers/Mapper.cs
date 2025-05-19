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

    public static partial StatesDTO MapToStateDto(Units entity);
    public static partial States MapToState (StatesDTO dto);
    public static partial IEnumerable<StatesDTO> MapToToStateDtoEnum(IEnumerable<States> entity);
}
