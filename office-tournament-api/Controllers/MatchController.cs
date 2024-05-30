using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;

namespace office_tournament_api.Controllers
{
    [Route("matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAccountService _accountService;
        public MatchController(DataContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        [HttpPost("{opponentId}")]
        public async Task<ActionResult<string>> CreateMatch(DTOMatchRequest dtoMatch)
        {
            try
            {
                return Ok("");
            }
            catch (Exception ex)
            {
                string error = $"CreateMatch failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
