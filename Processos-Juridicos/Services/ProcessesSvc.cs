using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessesSvc : IProcessesSvc
    {

        private readonly AppDbContext _context;
        public ProcessesSvc(AppDbContext context) {
            _context = context;
        }

        public Task<Processes> createProcess(Processes process)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteProcess(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Processes> editProcess(Processes process)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProcessesDTO>> getAllProcesses()
        {
            var processes = await _context.Processes.ToListAsync();
            return Mapper.MapToToProcessesEnum(processes);
        }

        public Task<Processes> getProcessById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
