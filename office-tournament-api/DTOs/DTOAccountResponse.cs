namespace office_tournament_api.DTOs
{
    public class DTOAccountResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public int MatchesWon {  get; set; }
    }
}
