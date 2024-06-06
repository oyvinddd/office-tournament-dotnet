using Microsoft.EntityFrameworkCore;
using office_tournament_api.office_tournament_db;
using System.Net.Mail;
using System.Security.Principal;

namespace office_tournament_api.Validators
{
    public class AccountValidator : IAccountValidator
    {
        private readonly DataContext _context;
        public AccountValidator(DataContext context) 
        {
            _context = context;
        }

        public async Task<AccountResult> IsValidAccount(Account account)
        {
            var accountResult = new AccountResult(true, new List<string>());
            bool validEmail = IsValidEmail(account.Email);
            bool emailExists = await DoesEmailExist(account.Email);
            bool userNameExists = await DoesUserNameExist(account.UserName);

            if (!validEmail)
            {
                string error = "Email provided is not a valid email";
                accountResult.IsValid = false;
                accountResult.Errors.Add(error);
            }

            if (emailExists)
            {
                string error = "An account with the same email exists";
                accountResult.IsValid = false;
                accountResult.Errors.Add(error);
            }

            if (userNameExists)
            {
                string error = "An account with the same UserName exists";
                accountResult.IsValid = false;
                accountResult.Errors.Add(error);
            }

            return accountResult;
        }

        private bool IsValidEmail(string email)
        {
            bool isValid = true;
            try
            {
                var mail = new MailAddress(email);
            }
            catch (Exception e)
            {
                isValid = false;
            }
            return isValid;
        }

        private async Task<bool> DoesEmailExist(string email)
        {
            bool exists = await _context.Accounts.Where(x => x.Email.Equals(email)).AnyAsync();
            return exists;
        }

        private async Task<bool> DoesUserNameExist(string userName)
        {
            bool exists = await _context.Accounts.Where(x => x.UserName.Equals(userName)).AnyAsync();

            return exists;
        }

        private async Task<bool> DoesTournamentExist(Guid tournamentId)
        {
            bool exists = true;
            Tournament tournament = await _context.Tournaments.FindAsync(tournamentId);

            if (tournament == null)
                exists = false; 

            return exists;
        }
    }
}
