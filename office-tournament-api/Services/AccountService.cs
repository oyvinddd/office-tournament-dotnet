using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace office_tournament_api.Services
{
    public class AccountService : IAccountService
    {
        private readonly DataContext _context;
        private readonly IAccountValidator _accountValidator;
        private readonly PasswordHandler _passwordHandler;
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly DTOMapper _mapper;
        public AccountService(DataContext context, IAccountValidator accountValidator, PasswordHandler passwordHandler, JwtTokenHandler jwtTokenHandler, DTOMapper mapper) 
        {
            _context = context;
            _accountValidator = accountValidator;
            _passwordHandler = passwordHandler;
            _jwtTokenHandler = jwtTokenHandler;
            _mapper = mapper;   
        }

        public async Task<DTOAccountResponse?> GetAccount(Guid id)
        {
            Account? account = await _context.Accounts.FindAsync(id);

            if (account == null)
                return null;

            DTOAccountResponse dtoAccount = _mapper.AccountDbToDto(account);
            return dtoAccount;
        } 

        public async Task<AccountResult?> Login(DTOAccountLoginRequest accountLogin)
        {
            AccountResult accountResult = new AccountResult(true, new List<string>());
            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts)
                .Where(x => x.UserName.Equals(accountLogin.UserName))
                .FirstOrDefaultAsync();

            if(account == null)
            {
                accountResult.Errors.Add($"Account with username '{accountLogin.UserName}' not found");
                return accountResult;
            }

            bool isValidPassword = _passwordHandler.VerifyPassword(accountLogin.Password, account.Password);

            if (!isValidPassword)
            {
                accountResult.Errors.Add("Password was not valid");
                return accountResult;
            }

            accountResult.Account = account;
            accountResult.Token = _jwtTokenHandler.CreateToken(account);

            return accountResult;
        } 

        public async Task<AccountResult> CreateAccount(DTOAccountRequest dtoAccount)
        {
            Account account = new Account();
            account.Email = dtoAccount.Email;
            account.UserName = dtoAccount.UserName;
            account.Password = _passwordHandler.CreatePassword(dtoAccount.Password);
            account.Email = dtoAccount.Email;
            account.TotalMatchesWon = 0;
            account.TotalMatchesPlayed = 0;
            account.CreatedDate = DateTime.UtcNow;
            account.UpdatedDate = DateTime.UtcNow;

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

            validationResult.Token = _jwtTokenHandler.CreateToken(account);
            validationResult.Account = account;

            return validationResult;
        }
    }
}
