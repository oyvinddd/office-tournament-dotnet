using Microsoft.IdentityModel.Tokens;
using office_tournament_api.office_tournament_db;

namespace office_tournament_api.DTOs
{
    public class DTOMapper
    {
        public DTOMapper() { }

        public DTOTournamentResponse TournamentDbToDto(Tournament tournament)
        {
            var dtoTournament = new DTOTournamentResponse();
            dtoTournament.Id = tournament.Id;   
            dtoTournament.Title = tournament.Title;

            if (!tournament.Participants.IsNullOrEmpty())
                dtoTournament.Accounts = (IList<DTOAccountResponse>)ListAccountDbToDto(tournament.Participants.ToList());

            return dtoTournament;
        }

        public List<DTOTournamentAccountResponse> ListAccountDbToDto(List<TournamentAccount> accounts)
        {
            var dtoAccounts = new List<DTOTournamentAccountResponse>();

            foreach (var account in accounts)
            {
                DTOTournamentAccountResponse dtoAccount = AccountDbToDto(account);
                dtoAccounts.Add(dtoAccount);
            }

            return dtoAccounts;
        }

        public DTOTournamentAccountResponse AccountDbToDto(TournamentAccount tournamentAccount)
        {
            Account account = tournamentAccount.Account;

            var dtoAccount = new DTOTournamentAccountResponse();
            dtoAccount.Id = tournamentAccount.Id;
            dtoAccount.UserName = account != null ? account.UserName : "";
            dtoAccount.Score = tournamentAccount.Score;
            dtoAccount.MatchesPlayed = tournamentAccount.MatchesPlayed;
            dtoAccount.MatchesWon = tournamentAccount.MatchesWon;

            return dtoAccount;
        }
    }
}
