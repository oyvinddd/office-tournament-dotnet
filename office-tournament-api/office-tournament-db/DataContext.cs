using Microsoft.EntityFrameworkCore;

namespace office_tournament_api.office_tournament_db
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Account>()
                .ToTable("Accounts")
                .HasOne(x => x.Tournament)
                .WithMany(x => x.Participants)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Tournament>()
                .ToTable("Tournaments")
                .HasOne(x => x.Admin)
                .WithOne(x => x.AdminTournament)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Match>()
                .ToTable("Matches")
                .HasOne(x => x.Tournament)
                .WithMany(x => x.Matches)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<Match>()
                .HasOne(x => x.Winner)
                .WithMany(x => x.MatchWins)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
               .Entity<Match>()
               .HasOne(x => x.Loser)
               .WithMany(x => x.MatchLosses)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
