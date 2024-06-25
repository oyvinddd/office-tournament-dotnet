using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface ITournamentService
    {
        Task<List<DTOTournamentResponse>> SearchTournaments(string query);
        Task<Tournament> GetTournament(Guid id);
        Task<TournamentAccount?> GetAdmin(Guid tournamentId);
        Task<TournamentResult?> GetActiveTournamentForAccount(HttpContext httpContext);
        Task<TournamentResult> JoinTournament(HttpContext httpContext, Guid tournamentId, DTOAccountJoinRequest joinInfo);
        Task<TournamentResult> LeaveTournament(HttpContext httpContext, Guid tournamentId);
        Task<TournamentResult> ResetTournaments();
        Task<TournamentResult> CreateTournament(HttpContext httpContext, DTOTournamentRequest dtoTournament);
    }
}
