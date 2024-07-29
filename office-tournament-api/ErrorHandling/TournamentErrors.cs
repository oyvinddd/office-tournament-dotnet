using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace office_tournament_api.ErrorHandling
{
    public class TournamentErrors
    {
        public static Error NotFound(Guid id)
        {
            return Error.NotFound("Tournaments.NotFound", $"The Tournament with id '{id}' was not found");
        }
        public static Error NoActiveTournament()
        {
            return Error.NotFound("Tournaments.NoActiveTournament", "No active tournament was found for the current Account");
        }

        public static Error NoAdminFound(Guid tournamentId)
        {
            return Error.NotFound("Tournaments.NoAdminFound", $"No admin TournamentAccount was found for Tournament with id '{tournamentId}'");
        }

        public static Error AlreadyJoined()
        {
            return Error.Validation("Tournaments.AlreadyJoined", "This account is already a participant of this Tournament");
        }

        public static Error InvalidCode(Guid tournamentId)
        {
            return Error.Validation("Tournaments.InvalidCode", $"Code supplied does not match code of Tournament with id = {tournamentId}");
        }

        public static Error ExistingTournamentsNotFound()
        {
            return Error.Validation("Tournaments.ExistingTournamentsNotFound", "No existing, active Tournaments were found");
        }

        public static Error DatabaseSaveFailure(string message)
        {
            return Error.Failure("Tournaments.DatabaseSaveFailure", message);
        }

        public static Error DuplicateAdmin(Guid accountId)
        {
            return Error.Validation("Tournaments.DuplicateAdmin", $"Account with id '{accountId}' is already admin for an active tournament");
        }

        public static Error SearchTournamentError(string message, string innerException)
        {
            return Error.Failure("Tournaments.SearchTournamentError", $"SearchTournament failed. Message: {message}. InnerException: {innerException}";);
        }

        public static Error GetTournamentError(string message, string innerException)
        {
            return Error.Failure("Tournaments.GetTournamentError", $"GetTournament failed. Message: {message}. InnerException: {innerException}";);
        }

        public static Error GetAdminError(string message, string innerException)
        {
            return Error.Failure("Tournaments.GetAdminError", $"GetAdmin failed. Message: {message}. InnerException: {innerException}";);
        }

        public static Error GetActiveTournamentForAccountError(string message, string innerException)
        {
            return Error.Failure("Tournaments.GetActiveTournamentForAccountError", $"GetActiveTournamentForAccount failed. Message: {message}. InnerException: {innerException}");
        }


    }
}
