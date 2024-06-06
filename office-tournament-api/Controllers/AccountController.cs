using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;
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
        private readonly IAccountService _accountService;
        public AccountController(DataContext context, IAccountService accountService) 
        {
            _context = context;
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Account>> Login(DTOAccountLoginRequest accountLogin)
        {
            try
            {
                AccountResult? accountResult = await _accountService.Login(accountLogin);

                if (accountResult.Account == null)
                    return NotFound(accountResult.Errors.FirstOrDefault());

                return Ok(accountResult.Account);
            }
            catch (Exception ex)
            {
                string error = $"Login failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        } 

        /// <summary>
        /// Gets an Account by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Account>> GetAccount(Guid id)
        {
            try
            {
                Account? account = await _accountService.GetAccount(id);

                if(account == null)
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
                AccountResult accountResult = await _accountService.CreateAccount(dtoAccount);

                if(!accountResult.IsValid)
                {
                    return BadRequest(accountResult.Errors);
                }

                string response = "A new Account was created";
                return Created("CreateAccount", response);
            }
            catch (Exception ex)
            {
                string error = $"CreateAccount failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
