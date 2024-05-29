using System.ComponentModel.DataAnnotations;

namespace office_tournament_api.DTOs
{
    public class DTOTournamentRequest
    {
        [MaxLength(50, ErrorMessage = "Title has max length 50")]
        public string Title { get; set; }
        public int ResetInterval { get; set; }
    }
}
