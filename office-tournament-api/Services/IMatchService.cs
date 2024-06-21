using office_tournament_api.DTOs;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface IMatchService
    {
        Task<TournamentResult> CreateMatch(HttpContext httpContext, DTOMatchRequest dtoMatch);
    }
}
