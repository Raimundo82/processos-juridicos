using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public  DbSet<AccidentType> Accident_types { get; set; }

    public DbSet<CrimeType> Crime_types { get; set; }

    public  DbSet<ProcessFile> Process_Files { get; set; }

    public  DbSet<HarmedOrCasualty> Harmed_or_casualties { get; set; }

    public  DbSet<Infringement> Infringements { get; set; }

    public  DbSet<ProcessType> Process_types { get; set; }

    public  DbSet<Process> Processes { get; set; }

    public  DbSet<Sentence> Sentences { get; set; }

    public  DbSet<State> States { get; set; }

    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Unit> Units { get; set; }
}
