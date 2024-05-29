using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    public class Account
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(TournamentId))]
        public Tournament Tournament { get; set; }
        public Guid TournamentId {  get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        public int Score { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesPlayed { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
