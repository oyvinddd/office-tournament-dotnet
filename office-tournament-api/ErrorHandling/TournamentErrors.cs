using System.Net.NetworkInformation;

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


    }
}
