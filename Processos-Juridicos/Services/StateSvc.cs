using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
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

        public async Task<IEnumerable<StateDto>> GetAllStates()
        {
            var states = await _context.States.ToListAsync();
            return Mapper.MapToToStateDtoEnum(states);
        }


         public async Task<StateDto> GetStateById(int id)
        {
            var state = await _context.States.FirstOrDefaultAsync(s => s.StateId == id)
        ?? throw new KeyNotFoundException($"O estado com o ID {id} não foi encontrado");
            return Mapper.MapToStateDto(state);

        }

        public async Task<StateDto> CreateState(StateDto state)
        {
            var existingState = await _context.States.FirstOrDefaultAsync(s => s.StateName == state.StateName);
            if (existingState != null)
            {
                throw new InvalidOperationException($"Já existe um estado com o nome '{state.StateName}'.");
            }

            var stateEntity = Mapper.MapToState(state);

            _context.States.Add(stateEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToStateDto(stateEntity);

        }


        public async Task<StateDto> EditState(StateDto state)
        {
            var duplicateState = await _context.States
                .Where(s => s.StateName == state.StateName && s.StateId != state.StateId)
                .FirstOrDefaultAsync();

            if (duplicateState != null)
            {
                throw new InvalidOperationException($"Já existe outro estado com o nome '{state.StateName}'.");
            }

            var stateEntity = Mapper.MapToState(state);
            _context.States.Entry(stateEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return state;

        }


        public async Task<bool> DeleteState(int id){
            var state = await _context.States.FirstOrDefaultAsync(x => x.StateId == id);
            if (state == null)
            {
                return false;
            }

            _context.States.Remove(state);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
