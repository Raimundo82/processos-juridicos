using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<Processos_DJContext> options) : base(options)
    {
    }

    public virtual DbSet<AccidentTypes> AccidentTypes { get; set; }

    public virtual DbSet<Files> Files { get; set; }

    public virtual DbSet<HarmedOrCasualties> HarmedOrCasualties { get; set; }

    public virtual DbSet<Infringements> Infringements { get; set; }

    public virtual DbSet<ProcessTypes> ProcessTypes { get; set; }

    public virtual DbSet<Processes> Processes { get; set; }

    public virtual DbSet<Sentences> Sentences { get; set; }

    public virtual DbSet<States> States { get; set; }

    public virtual DbSet<Units> Units { get; set; }
}
