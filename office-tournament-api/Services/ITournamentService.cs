using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface ITournamentService
    {
        Task<List<DTOTournamentResponse>> SearchTournaments(string query);
        Task<(Result, Tournament?)> GetTournament(Guid id);
        Task<TournamentAccount?> GetAdmin(Guid tournamentId);
        Task<(Result, Tournament?)> GetActiveTournamentForAccount(HttpContext httpContext);
        Task<Result> JoinTournament(HttpContext httpContext, Guid tournamentId, DTOAccountJoinRequest joinInfo);
        Task<Result> LeaveTournament(HttpContext httpContext, Guid tournamentId);
        Task<Result> ResetTournaments();
        Task<(Result, Tournament?)> CreateTournament(HttpContext httpContext, DTOTournamentRequest dtoTournament);
    }
}
