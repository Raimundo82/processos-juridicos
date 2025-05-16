using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IUnitSvc
    {
        Task<List<Units>> getAllUnits();
        Task<Units> getUnitById(int id);
        Task<Units> createUnit(Units unit);
        Task<Units> editUnit(Units unit);
        Task<bool> deleteUnit(int id);
    }
}
