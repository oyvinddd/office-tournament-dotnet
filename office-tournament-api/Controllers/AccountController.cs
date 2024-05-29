using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using System.Net.Mail;

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
        
        public async Task<ActionResult> CreateAccount(DTOAccountRequest dtoAccount)
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

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return Created("CreateAccount", DateTime.Now);
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
