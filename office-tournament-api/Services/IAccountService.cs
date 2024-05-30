using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public interface IAccountService
    {
        Task<Account?> GetAccount(Guid id);
        Task<AccountResult> CreateAccount(DTOAccountRequest dtoAccount);
    }
}
