using Microsoft.Identity.Client;
using office_tournament_api.office_tournament_db;

namespace office_tournament_api.ErrorHandling
{
    public class TournamentAccountErrors
    {
        public static Error NotFound(Guid id)
        {
            return Error.NotFound("TournamentAccounts.NotFound", $"The Account with id '{id}' was not found");
        }

        public static Error NotFoundByTournamentAndAccount(Guid tournamentId, Guid accountId )
        {
            return Error.NotFound("TournamentAccounts.NotFoundByTournamentAndAccount", $"No TournamentAccount was found for Account with id '{accountId}' and Tournament with id '{tournamentId}'");
        }

        public static Error ExistingUsername()
        {
            Error newError = Error.Validation("TournamentAccounts.ExistingUsername", "An account with the same username already exists");

            return newError;
        }

        public static Error DatabaseSaveFailure(string message)
        {
            Error newError = Error.Failure("TournamentAccounts.DatabaseFailure", $"Insert of TournamentAccount failed during database save. Message: {message}");

            return newError;
        }
        public static Error DatabaseSaveFailureLeaveTournament(Guid tournamentId, Guid accountId, string message)
        {
            Error newError = Error.Failure("TournamentAccounts.DatabaseFailure", $"There was an error when trying to remove Account with id = {accountId} from Tournament with id = {tournamentId}. Message: {message}. ");

            return newError;
        }
    }
}
