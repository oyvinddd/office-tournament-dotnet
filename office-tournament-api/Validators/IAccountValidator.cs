using office_tournament_api.ErrorHandling;
using office_tournament_api.office_tournament_db;

namespace office_tournament_api.Validators
{
    public interface IAccountValidator
    {
        Task<Result> IsValidAccount(Account account);
    }
}
