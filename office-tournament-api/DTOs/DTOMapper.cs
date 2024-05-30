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
                dtoTournament.Accounts = ListAccountDbToDto(tournament.Participants.ToList());

            return dtoTournament;
        }

        public List<DTOAccountResponse> ListAccountDbToDto(List<Account> accounts)
        {
            var dtoAccounts = new List<DTOAccountResponse>();

            foreach (var account in accounts)
            {
                DTOAccountResponse dtoAccount = AccountDbToDto(account);
                dtoAccounts.Add(dtoAccount);
            }

            return dtoAccounts;
        }

        public DTOAccountResponse AccountDbToDto(Account account)
        {
            var dtoAccount = new DTOAccountResponse();
            dtoAccount.Id = account.Id;
            dtoAccount.UserName = account.UserName;
            dtoAccount.Score = account.Score;
            dtoAccount.MatchesPlayed = account.MatchesPlayed;
            dtoAccount.MatchesWon = account.MatchesWon;

            return dtoAccount;
        }
    }
}
