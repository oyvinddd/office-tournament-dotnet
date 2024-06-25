using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    [Index(nameof(TournamentId), nameof(AccountId), IsUnique = true)]
    public class TournamentAccount
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(TournamentId))]
        public Tournament Tournament { get; set; }
        public Guid TournamentId { get; set; }
        [ForeignKey(nameof(AdminTournamentId))]
        public Tournament? AdminTournament { get; set; }
        public Guid? AdminTournamentId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
        public float Score { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesPlayed { get; set; }
        public DateTime UpdatedDate { get; set; }
        public IList<Match> MatchWins { get; set; }
        public IList<Match> MatchLosses { get; set; }

        public TournamentAccount() { }

        public TournamentAccount(Tournament tournament, Guid tournamentId, Tournament? adminTournament, Guid? adminTournamentId, Account account, Guid accountId, 
            float score, int matchesWon, int matchesPlayed, DateTime updatedDate)
        {
            Tournament = tournament;
            TournamentId = tournamentId;
            AdminTournament = adminTournament;
            AdminTournamentId = adminTournamentId;
            Account = account;
            AccountId = accountId;
            Score = score;
            MatchesWon = matchesWon;
            MatchesPlayed = matchesPlayed;
            UpdatedDate = updatedDate;
        }
    }
}
