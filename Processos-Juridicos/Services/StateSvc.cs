using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class StateSvc : IStateSvc
    {
        private readonly AppDbContext _context;

        public StateSvc(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StatesDTO>> getAllStates()
        {
            var states = await _context.States.ToListAsync();
            return Mapper.MapToToStateDtoEnum(states);
        }

         public Task<States> getStateById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<States> createState(States state)
        {
            throw new NotImplementedException();
        }


        public Task<States> editState(States state)
        {
            throw new NotImplementedException();
        }


        public Task<bool> deleteState(int id)
        {
            throw new NotImplementedException();
        }
    }
}
