using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;
using System.Net.Mail;
using System.Security.Principal;

namespace office_tournament_api.Controllers
{
    [Route("accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        public AccountController(DataContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Gets an Account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(Guid id)
        {
            try
            {
                Account? account = await _context.Accounts.FindAsync(id);

                if (account == null)
                {
                    string error = $"Account with id = {id} was not found";
                    return NotFound(error);
                }

                return Ok(account);
            }catch (Exception ex)
            {
                string error = $"GetAccount failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }

        /// <summary>
        /// Creates a new Account
        /// </summary>
        /// <param name="dtoAccount"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateAccount(DTOAccountRequest dtoAccount)
        {
            try
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

                AccountResult validationResult = await IsValidAccount(account);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();

                string response = "A new Account was created";
                return Created("CreateAccount", response);
            }
            catch (Exception ex)
            {
                string error = $"CreateAccount failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }

        private async Task<AccountResult> IsValidAccount(Account account)
        {
            var accountResult = new AccountResult(true, new List<string>());
            bool validEmail = IsValidEmail(account.Email);
            bool emailExists = await DoesEmailExist(account.Email);
            bool userNameExists = await DoesUserNameExist(account.UserName);

            if(!validEmail)
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
            catch(Exception e)
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
    }
}
