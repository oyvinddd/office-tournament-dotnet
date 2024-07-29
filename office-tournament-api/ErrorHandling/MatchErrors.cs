using static System.Runtime.InteropServices.JavaScript.JSType;

namespace office_tournament_api.ErrorHandling
{
    public class MatchErrors
    {
        public static Error NotFoundOpponentAccount(Guid opponentId)
        {
            return Error.NotFound("Matches.NotFoundOpponentAccount", $"Opponent Account with id = {opponentId} was not found");
        }

        public static Error NotFoundOpponentTournamentAccount(Guid opponentTournamentAccountId, Guid tournamentId)
        {
            return Error.NotFound("Matches.NotFoundOpponentTournamentAccount", $"Opponent TournamentAccount for Account with id = {opponentTournamentAccountId} and Tournament with id = {tournamentId} was not found");
        }

        public static Error DatabaseSaveFailure(string message)
        {
            return Error.Failure("Matches.DatabaseSaveFailure", message);
        }
    }
}
