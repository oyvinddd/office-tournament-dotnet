using Microsoft.IdentityModel.Tokens;
using office_tournament_api.office_tournament_db;

namespace office_tournament_api.DTOs
{
    public class DTOMapper
    {
        public DTOMapper() { }

        public List<DTOTournamentResponse> ListTournamentDbToDto(List<Tournament> tournaments)
        {
            var dtoTournaments = new List<DTOTournamentResponse>();

            foreach (var tournament in tournaments)
            {
                DTOTournamentResponse dtoTournament = TournamentDbToDto(tournament);
                dtoTournaments.Add(dtoTournament);
            }

            return dtoTournaments;
        }

        public DTOTournamentResponse TournamentDbToDto(Tournament tournament)
        {
            var dtoTournament = new DTOTournamentResponse();
            dtoTournament.Id = tournament.Id;
            dtoTournament.AdminId = tournament.AdminId;
            dtoTournament.Title = tournament.Title;
            dtoTournament.IsActive = tournament.IsActive;

            if (!tournament.Participants.IsNullOrEmpty())
                dtoTournament.TournamentAccounts = (IList<DTOTournamentAccountResponse>)ListTournamentAccountDbToDto(tournament.Participants.ToList());

            return dtoTournament;
        }

        public List<DTOTournamentAccountResponse> ListTournamentAccountDbToDto(List<TournamentAccount> accounts)
        {
            var dtoAccounts = new List<DTOTournamentAccountResponse>();

            foreach (var account in accounts)
            {
                DTOTournamentAccountResponse dtoAccount = TournamentAccountDbToDto(account);
                dtoAccounts.Add(dtoAccount);
            }

            return dtoAccounts;
        }

        public DTOTournamentAccountResponse TournamentAccountDbToDto(TournamentAccount tournamentAccount)
        {
            Account? account = tournamentAccount.Account;

            var dtoAccount = new DTOTournamentAccountResponse();
            dtoAccount.Id = tournamentAccount.Id;
            dtoAccount.AccountId = account != null ? account.Id : null;
            dtoAccount.UserName = account != null ? account.UserName : "";
            dtoAccount.Score = tournamentAccount.Score;
            dtoAccount.MatchesPlayed = tournamentAccount.MatchesPlayed;
            dtoAccount.MatchesWon = tournamentAccount.MatchesWon;

            return dtoAccount;
        }

        public DTOAccountResponse AccountDbToDto(Account account)
        {
            var dtoAccount = new DTOAccountResponse();
            dtoAccount.Id = account.Id;
            dtoAccount.Email = account.Email;
            dtoAccount.UserName = account.UserName;
            dtoAccount.TotalMatchesWon = account.TotalMatchesWon;
            dtoAccount.TotalMatchesPlayed = account.TotalMatchesPlayed;
            dtoAccount.CreatedDate = account.CreatedDate;
            dtoAccount.UpdatedDate = account.UpdatedDate;

            if(!account.TournamentAccounts.IsNullOrEmpty())
            {
                dtoAccount.TournamentAccounts = ListTournamentAccountDbToDto(account.TournamentAccounts.ToList());
            }else
            {
                account.TournamentAccounts = [];
            }

            return dtoAccount;
        }
    }
}
