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
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
        public float Score { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesPlayed { get; set; }
        public DateTime UpdatedDate { get; set; }
        public IList<Match> MatchWins { get; set; }
        public IList<Match> MatchLosses { get; set; }
    }
}
