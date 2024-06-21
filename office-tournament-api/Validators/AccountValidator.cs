using Microsoft.EntityFrameworkCore;
using office_tournament_api.ErrorHandling;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using System.Net.Mail;
using System.Security.Principal;

namespace office_tournament_api.Validators
{
    public class AccountValidator : IAccountValidator
    {
        private readonly DataContext _context;
        private readonly PasswordHandler _passwordHandler;
        public AccountValidator(DataContext context, PasswordHandler passwordHandler) 
        {
            _context = context;
            _passwordHandler = passwordHandler;
        }

        public async Task<Result> IsValidAccount(Account account)
        {
            bool isValid = true;
            List<Error> errors = new List<Error>();
            bool validEmail = IsValidEmail(account.Email);
            bool emailExists = await DoesEmailExist(account.Email);
            bool userNameExists = await DoesUserNameExist(account.UserName);

            if (!validEmail)
            {
                isValid = false;
                errors.Add(AccountErrors.InvalidEmail());
            }

            if (emailExists)
            {
                isValid = false;
                errors.Add(AccountErrors.ExistingEmail());
            }

            if (userNameExists)
            {
                isValid = false;
                errors.Add(AccountErrors.ExistingUsername());
            }

            if (!isValid)
                return Result.Failure(errors);

            return Result.Success();
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
