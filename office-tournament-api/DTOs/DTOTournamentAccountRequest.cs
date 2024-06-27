using office_tournament_api.office_tournament_db;
using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.DTOs
{
    public class DTOTournamentAccountRequest
    {
        public Guid TournamentId { get; set; }
        public Guid AccountId { get; set; }
    }
}
