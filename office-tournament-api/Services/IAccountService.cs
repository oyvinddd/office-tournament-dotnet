using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface IAccountService
    {
        Task<AccountResult?> Login(DTOAccountLoginRequest accountLogin);
        Task<(Result, DTOAccountResponse?)> GetAccount(Guid id);
        Task<AccountResult> CreateAccount(DTOAccountRequest dtoAccount);
    }
}
