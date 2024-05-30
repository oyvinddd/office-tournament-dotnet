using office_tournament_api.DTOs;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface ITournamentService
    {
        Task<TournamentResult> JoinTournament(HttpContext httpContext, Guid tournamentId, DTOAccountJoinRequest joinInfo);
        Task<TournamentResult> CreateTournament(DTOTournamentRequest dtoTournament);
    }
}
