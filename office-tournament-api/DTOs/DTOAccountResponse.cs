using System.ComponentModel.DataAnnotations;

namespace office_tournament_api.DTOs
{
    public class DTOAccountResponse
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        public int TotalMatchesWon { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public IList<DTOTournamentAccountResponse> TournamentAccounts { get; set; }
    }
}
