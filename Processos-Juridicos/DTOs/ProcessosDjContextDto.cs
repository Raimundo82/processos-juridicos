#nullable disable
using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

public partial class ProcessosDjContextDto(DbContextOptions<ProcessosDjContextDto> options) : DbContext(options)
{
    public virtual DbSet<AccidentTypeDto> Accident_types { get; set; }

    public virtual DbSet<ProcessFileDto> Files { get; set; }

    public virtual DbSet<HarmedOrCasualtyDto> Harmed_or_casualties { get; set; }

    public virtual DbSet<InfringementDto> Infringements { get; set; }

    public virtual DbSet<ProcessTypeDto> Process_types { get; set; }

    public virtual DbSet<ProcessDto> Processes { get; set; }

    public virtual DbSet<SentenceDto> Sentences { get; set; }

    public virtual DbSet<StateDto> States { get; set; }

    public virtual DbSet<UnitDto> Units { get; set; }
}