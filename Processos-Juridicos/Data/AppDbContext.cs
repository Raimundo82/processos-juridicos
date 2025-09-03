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

    public DbSet<UnitCommander> UnitCommanders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Unit>()
       .HasMany(u => u.ResponsibleUsers)
       .WithMany(u => u.UnitsResponsibleFor)
       .UsingEntity<UnitCommander>(
           j => j
               .HasOne(uc => uc.User)
               .WithMany(u => u.UnitCommanders)
               .HasForeignKey(uc => uc.UserNii),
           j => j
               .HasOne(uc => uc.Unit)
               .WithMany(u => u.UnitCommanders)
               .HasForeignKey(uc => uc.UnitId),
           j =>
           {
               j.HasKey(uc => new { uc.UnitId, uc.UserNii });
               j.ToTable("Unit_commanders"); // match your actual table name
           });

        modelBuilder.Entity<StateTransition>()
            .HasOne(st => st.FromState)
            .WithMany()
            .HasForeignKey(st => st.FromStateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StateTransition>()
            .HasOne(st => st.ToState)
            .WithMany()
            .HasForeignKey(st => st.ToStateId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<Role> Roles { get; set; }
}
