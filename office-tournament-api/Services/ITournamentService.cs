using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface ITournamentService
    {
        Task<Tournament> GetTournament(Guid id);
        Task<ValidationResult> JoinTournament(HttpContext httpContext, Guid tournamentId, DTOAccountJoinRequest joinInfo);
        Task<ValidationResult> LeaveTournament(HttpContext httpContext, Guid tournamentId);
        Task<ValidationResult> CreateTournament(HttpContext httpContext, DTOTournamentRequest dtoTournament);
    }
}
