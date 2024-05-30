using office_tournament_api.office_tournament_db;

namespace office_tournament_api.Validators
{
    public interface IAccountValidator
    {
        Task<AccountResult> IsValidAccount(Account account);
    }
}
