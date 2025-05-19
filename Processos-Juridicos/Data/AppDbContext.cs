using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public  DbSet<Accident_types> Accident_types { get; set; }

    public  DbSet<Files> Files { get; set; }

    public  DbSet<Harmed_or_casualties> Harmed_or_casualties { get; set; }

    public  DbSet<Infringements> Infringements { get; set; }

    public  DbSet<Process_types> Process_types { get; set; }

    public  DbSet<Processes> Processes { get; set; }

    public  DbSet<Sentences> Sentences { get; set; }

    public  DbSet<States> States { get; set; }

    public  DbSet<Units> Units { get; set; }
    public  DbSet<Sectors> Sectors { get; set; }
}
