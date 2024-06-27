using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;

namespace office_tournament_api.Services
{
    public interface IPushTokenService
    {
        Task<Result> CreatePushToken(HttpContext httpContext, DTOPushTokenRequest dtoToken);
    }
}
