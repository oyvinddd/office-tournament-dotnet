using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;
using System.Security.Principal;

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

        public async Task<(Result, DTOAccountResponse?)> GetAccount(Guid id)
        {
            Account? account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                List<Error> errors = new List<Error> { AccountErrors.NotFound(id) };
                return (Result.Failure(errors), null);
            }

            DTOAccountResponse dtoAccount = _mapper.AccountDbToDto(account);
            return (Result.Success(), dtoAccount);
        } 

        public async Task<(Result, DTOAccountInfoResponse?)> Login(DTOAccountLoginRequest accountLogin)
        {
            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts)
                .Where(x => x.UserName.Equals(accountLogin.UserName))
                .FirstOrDefaultAsync();

            if (account == null)
            {
                return (Result.Failure(new List<Error> { AccountErrors.UserNameNotFound(accountLogin.UserName) }), null);
            }

            bool isValidPassword = _passwordHandler.VerifyPassword(accountLogin.Password, account.Password);

            if(!isValidPassword)
            {
                return (Result.Failure(new List<Error> { AccountErrors.InvalidPassword() }), null);
            }

            string token = _jwtTokenHandler.CreateToken(account);
            DTOAccountResponse dtoAccount = _mapper.AccountDbToDto(account);
            var dtoAccountInfo = new DTOAccountInfoResponse(dtoAccount, token);
            return (Result.Success(), dtoAccountInfo);
        } 

        public async Task<(Result, DTOAccountInfoResponse?)> CreateAccount(DTOAccountRequest dtoAccount)
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

            Result validationResult = await _accountValidator.IsValidAccount(account);

            if (validationResult.IsFailure)
            {
                return (validationResult, null);
            }

            try
            {
                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return (Result.Failure(new List<Error> { AccountErrors.DatabaseFail(e.Message) }), null);
            }

            string token = _jwtTokenHandler.CreateToken(account);
            DTOAccountResponse dtoAccountResponse = _mapper.AccountDbToDto(account);
            var dtoAccountInfo = new DTOAccountInfoResponse(dtoAccountResponse, token);
            return (Result.Success(), dtoAccountInfo);
        }
    }
}
