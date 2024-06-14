using Microsoft.EntityFrameworkCore;

namespace office_tournament_api.office_tournament_db
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TournamentAccount> TournamentAccounts { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Account>()
                .ToTable("Accounts");

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

            modelBuilder
                .Entity<TournamentAccount>()
                .ToTable("TournamentAccounts")
                .HasOne(x => x.Tournament)
                .WithMany(x => x.Participants)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<TournamentAccount>()
                .HasOne(x => x.Account)
                .WithMany(x => x.TournamentAccounts)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder
                .Entity<Tournament>()
                .ToTable("Tournaments");
        }
    }
}
