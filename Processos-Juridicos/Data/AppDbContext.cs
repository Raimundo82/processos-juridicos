using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AccidentType> AccidentTypes { get; set; }

    public DbSet<CrimeType> CrimeTypes { get; set; }

    public DbSet<ProcessFile> ProcessFiles { get; set; }

    public DbSet<HarmedOrCasualty> HarmedOrCasualties { get; set; }

    public DbSet<Infringement> Infringements { get; set; }

    public DbSet<ProcessType> ProcessTypes { get; set; }

    public DbSet<Process> Processes { get; set; }

    public DbSet<Sentence> Sentences { get; set; }

    public DbSet<ProcessState> States { get; set; }

    public DbSet<Unit> Units { get; set; }

    public DbSet<MilitarySecurity> MilitarySecurities { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<StateTransition> StateTransitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StateTransition>()
            .HasOne(st => st.FromState)
            .WithMany()
            .HasForeignKey(st => st.FromStateId)
            .OnDelete(DeleteBehavior.Restrict); // prevent cascade from this side

        modelBuilder.Entity<StateTransition>()
            .HasOne(st => st.ToState)
            .WithMany()
            .HasForeignKey(st => st.ToStateId)
            .OnDelete(DeleteBehavior.Cascade); // keep cascade (or use Restrict here instead)
    }

    public DbSet<Role> Roles { get; set; }
}
