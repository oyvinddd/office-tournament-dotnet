using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
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
        private readonly DTOMapper _mapper;
        public AccountController(DataContext context, IAccountService accountService, DTOMapper mapper) 
        {
            _context = context;
            _accountService = accountService;
            _mapper = mapper;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="accountLogin"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(DTOAccountInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOAccountInfoResponse>> Login(DTOAccountLoginRequest accountLogin)
        {
            try
            {
                AccountResult? accountResult = await _accountService.Login(accountLogin);

                if (accountResult.Account == null)
                    return NotFound(accountResult.Errors.FirstOrDefault());

                DTOAccountResponse dtoAccount = _mapper.AccountDbToDto(accountResult.Account);
                var dtoAccountInfo = new DTOAccountInfoResponse(dtoAccount, accountResult.Token);

                return Ok(dtoAccountInfo);
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
        [ProducesResponseType(typeof(DTOAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<DTOAccountResponse>> GetAccount(Guid id)
        {
            try
            {
                (Result result, DTOAccountResponse? dtoAccount) = await _accountService.GetAccount(id);

                if(result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                return Ok(dtoAccount);
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
        [ProducesResponseType(typeof(DTOAccountInfoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOAccountInfoResponse>> CreateAccount(DTOAccountRequest dtoAccount)
        {
            try
            {
                AccountResult accountResult = await _accountService.CreateAccount(dtoAccount);

                if(!accountResult.IsValid)
                {
                    return BadRequest(accountResult.Errors);
                }

                DTOAccountResponse dtoAccountResponse = _mapper.AccountDbToDto(accountResult.Account);
                var dtoAccountInfo = new DTOAccountInfoResponse(dtoAccountResponse, accountResult.Token);
                return Created("CreateAccount", dtoAccountResponse);
            }
            catch (Exception ex)
            {
                string error = $"CreateAccount failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
