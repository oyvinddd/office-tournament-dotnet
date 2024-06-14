using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;
using office_tournament_api.Validators;

namespace office_tournament_api.Controllers
{
    [Route("matches")]
    [ApiController]
    [Authorize]
    public class MatchController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMatchService _matchService;
        public MatchController(DataContext context, IMatchService matchService)
        {
            _context = context;
            _matchService = matchService;
        }

        /// <summary>
        /// Creates a new Match and calculates new elo rating scores for the players
        /// </summary>
        /// <param name="dtoMatch"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateMatch(DTOMatchRequest dtoMatch)
        {
            try
            {
                ValidationResult validationResult = await _matchService.CreateMatch(HttpContext, dtoMatch);

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                return Created("CreateMatch", "");
            }
            catch (Exception ex)
            {
                string error = $"CreateMatch failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
