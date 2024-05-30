namespace office_tournament_api.DTOs
{
    public class DTOMatchRequest
    {
        public Guid TournamentId { get; set; }
        public Guid OpponentId { get; set; }

    }
}
