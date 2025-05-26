using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessTypesSvc : IProcessTypeSvc
    {
        private readonly AppDbContext _context;

        public ProcessTypesSvc(AppDbContext context)
        {
            _context = context;
        }


        public Task<ProcessType> createProcessType(ProcessType type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteProcessType(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessType> editProcessType(ProcessType type)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProcessTypeDto>> getAllProcessTypes()
        {
            var types = await _context.Process_types.ToListAsync();
            return Mapper.MapToToProcessTypeDtoEnum(types);
        }

        public Task<ProcessType> getProcessTypeById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
