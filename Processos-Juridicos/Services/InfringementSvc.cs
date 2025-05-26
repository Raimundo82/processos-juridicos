using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class InfringementSvc : IInfringementSvc
    {
        private readonly AppDbContext _context;

        public InfringementSvc(AppDbContext context) { 
            _context = context;
        }
        public Task<Infringement> createInfringement(Infringement infringement)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteInfringement(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Infringement> editInfringement(Infringement infringement)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<InfringementDto>> getAllInfringements()
        {
            var infringements = await _context.Infringements.ToListAsync();
            return Mapper.MapToToInfringementsEnum(infringements);
        }

        public Task<Infringement> getInfringementById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
