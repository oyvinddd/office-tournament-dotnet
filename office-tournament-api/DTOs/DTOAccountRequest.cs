using System.ComponentModel.DataAnnotations;

namespace office_tournament_api.DTOs
{
    public class DTOAccountRequest
    {
        public Guid? TournamentId { get; set; }
        public Guid? AdminTournamentId { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
    }
}
