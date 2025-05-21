using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class InfringementsSvc : IInfringementsSvc
    {
        private readonly AppDbContext _context;

        public InfringementsSvc(AppDbContext context) { 
            _context = context;
        }
        public Task<Infringements> createInfringement(Infringements infringement)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteInfringement(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Infringements> editInfringement(Infringements infringement)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InfringementsDTO>> getAllInfringements()
        {
            var infringements = await _context.Infringements.ToListAsync();
            return Mapper.MapToToInfringementsEnum(infringements);
        }

        public Task<Infringements> getInfringementById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
