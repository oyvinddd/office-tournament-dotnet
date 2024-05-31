using office_tournament_api.DTOs;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface IMatchService
    {
        Task<ValidationResult> CreateMatch(HttpContext httpContext, DTOMatchRequest dtoMatch);
    }
}
