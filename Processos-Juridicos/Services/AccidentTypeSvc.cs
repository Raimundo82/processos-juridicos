using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class AccidentTypeSvc : IAccidentTypeSvc
    {
        private readonly AppDbContext _context;

        public AccidentTypeSvc(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccidentTypeDto>> getAllAccidents()
        {
            var accidents = await _context.Accident_types.ToListAsync();
            return Mapper.MapToAccidentTypeEnum(accidents);
        }

        public Task<AccidentType> geAccidentById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<AccidentType> createAccident(AccidentType type)
        {
            throw new NotImplementedException();
        }

        public Task<AccidentType> editAccident(AccidentType type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteAccident(int id)
        {
            throw new NotImplementedException();
        }
    }
}
