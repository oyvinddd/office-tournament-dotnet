using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;
using System.Security.Principal;

namespace office_tournament_api.Services
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IAccountValidator _accountValidator;
        public AccountService(DataContext context, IAccountValidator accountValidator) 
        {
            _context = context;
            _accountValidator = accountValidator;
        }

        public async Task<Account?> GetAccount(Guid id)
        {
            Account? account = await _context.Accounts.FindAsync(id);

            if (account == null)
                return null;

            return account;
        } 

        public async Task<AccountResult> CreateAccount(DTOAccountRequest dtoAccount)
        {
            Account account = new Account();
            account.TournamentId = dtoAccount.TournamentId;
            account.AdminTournamentId = dtoAccount.AdminTournamentId;
            account.Email = dtoAccount.Email;
            account.UserName = dtoAccount.UserName;
            account.Email = dtoAccount.Email;
            account.Score = 1600;
            account.MatchesWon = 0;
            account.MatchesPlayed = 0;
            account.CreateDate = DateTime.UtcNow;

            AccountResult validationResult = await _accountValidator.IsValidAccount(account);

            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            try
            {
                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                validationResult.IsValid = false;
                validationResult.Errors.Add(e.Message);
            }

            return validationResult;
        }
    }
}
