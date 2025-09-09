using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;

namespace Processos_Juridicos.Services.DomainData;

public class InfringementSvc(AppDbContext context) : IInfringementSvc
{
    private readonly AppDbContext _context = context;

    public async Task<InfringementDto> CreateInfringement(InfringementDto infringement)
    {
        Infringement infringementEntity = Mapper.MapToInfringements(infringement);

        _context.Infringements.Add(infringementEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToInfringementsDto(infringementEntity);
    }

    public async Task<bool> DeleteInfringement(int? id)
    {
        Infringement? infringement = await _context.Infringements.FindAsync(id);

        if (infringement == null)
        {
            return false;
        }

        _context.Infringements.Remove(infringement);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<InfringementDto> EditInfringement(InfringementDto infringement)
    {
        Infringement infringementEntity = Mapper.MapToInfringements(infringement);
        _context.Infringements.Entry(infringementEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToInfringementsDto(infringementEntity);
    }

    public async Task<IEnumerable<InfringementDto>> GetAllInfringements()
    {
        List<Infringement> infringements = await _context.Infringements.ToListAsync();
        return Mapper.MapToToInfringementsEnum(infringements);
    }

    public async Task<List<InfringementDto>> GetAllInfringementsByProcessId(int? id)
    {
        return await _context.Processes
            .Where(p => p.ProcessId == id)
            .SelectMany(p => p.Infringements)
            .Select(i => Mapper.MapToInfringementsDto(i))
            .ToListAsync();
    }


    public async Task<InfringementDto> GetInfringementById(int? id)
    {
        Infringement? infringement = await _context.Infringements.FindAsync(id);
        return infringement != null ? Mapper.MapToInfringementsDto(infringement) : throw new EntityNotFoundException("Infringement not found");
    }
}
