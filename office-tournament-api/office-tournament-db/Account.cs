using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Account
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(TournamentId))]
        public Tournament? Tournament { get; set; }
        public Guid? TournamentId {  get; set; }
        [ForeignKey(nameof(AdminTournamentId))]
        public Tournament? AdminTournament { get; set; }
        public Guid? AdminTournamentId { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public float Score { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesPlayed { get; set; }
        public DateTime CreateDate { get; set; }
        public IList<Match> MatchWins { get; set; }
        public IList<Match> MatchLosses { get; set; }
    }
}
