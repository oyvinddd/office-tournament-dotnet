﻿namespace office_tournament_api.DTOs
{
    public class DTOTournamentResponse
    {
        public Guid Id { get; set; }
        public Guid? AdminId { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public IList<DTOTournamentAccountResponse> TournamentAccounts { get; set; }
    }
}
