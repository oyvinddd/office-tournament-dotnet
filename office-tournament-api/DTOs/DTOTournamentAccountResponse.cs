namespace office_tournament_api.DTOs
{
    public class DTOTournamentAccountResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public float Score { get; set; }
        public int MatchesWon { get; set; }
        public int MatchesPlayed { get; set; }
    }
}
