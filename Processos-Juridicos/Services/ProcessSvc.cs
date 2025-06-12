using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessSvc(AppDbContext context) : IProcessSvc
    {
        private readonly AppDbContext _context = context;

        public Task<ProcessDto> CreateProcess(ProcessDto process)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProcess(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessDto> EditProcess(ProcessDto process)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProcessDto>> GetAllProcesses()
        {
            var processes = await _context.Processes.ToListAsync();
            return Mapper.MapToToProcessesEnum(processes);
        }

        public Task<ProcessDto> GetProcessById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
