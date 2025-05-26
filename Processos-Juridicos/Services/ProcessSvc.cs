using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessSvc : IProcessSvc
    {

        private readonly AppDbContext _context;
        public ProcessSvc(AppDbContext context) {
            _context = context;
        }

        public Task<Process> createProcess(Process process)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteProcess(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Process> editProcess(Process process)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProcessDto>> getAllProcesses()
        {
            var processes = await _context.Processes.ToListAsync();
            return Mapper.MapToToProcessesEnum(processes);
        }

        public Task<Process> getProcessById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
