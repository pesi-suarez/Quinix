using Microsoft.EntityFrameworkCore;
using Quinix.Model;

//TODO: Mirar los warnings del EF. Tal vez es la versión de .NET Core?
namespace Quinix.Data
{
    public class QuinixDbContext : DbContext
    {
        const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=Quinix;Trusted_Connection=True;";

        public QuinixDbContext() : base() { }

        public QuinixDbContext(DbContextOptions<QuinixDbContext> options) : base(options) { }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }

        //TODO: Ver cómo hago para que en los tests unitarios se use una BD de test.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Division>().Property(d => d.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Division>().Property(d => d.Number).IsRequired();

            modelBuilder.Entity<Season>().Property(s => s.StartYear).IsRequired();
            modelBuilder.Entity<Season>().Property(s => s.EndYear).IsRequired();

            modelBuilder.Entity<Team>().Property(t => t.Name).IsRequired().HasMaxLength(50);

            modelBuilder.Entity<Match>().Property(m => m.MatchDay).IsRequired();
            modelBuilder.Entity<Match>().Property(m => m.HomeTeamGoals).IsRequired();
            modelBuilder.Entity<Match>().Property(m => m.AwayTeamGoals).IsRequired();

            modelBuilder.Entity<Division>().HasIndex(d => d.Name).IsUnique();
            modelBuilder.Entity<Season>().HasIndex(s => new { s.StartYear, s.EndYear}).IsUnique();
            modelBuilder.Entity<Team>().HasIndex(t => t.Name).IsUnique();
            modelBuilder.Entity<Match>().HasIndex(m => new { m.DivisionId, m.SeasonId, m.HomeTeamId, m.AwayTeamId}).IsUnique();
        }
    }
}
