namespace office_tournament_api.DTOs
{
    public class DTOTournamentResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IList<DTOAccountResponse> Accounts { get; set; }
    }
}
