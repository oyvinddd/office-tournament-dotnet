using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateMatch(DTOMatchRequest dtoMatch)
        {
            try
            {
                (Result result, string? message) = await _matchService.CreateMatch(HttpContext, dtoMatch);

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                return Created("CreateMatch", message);
            }
            catch (Exception ex)
            {
                string error = $"CreateMatch failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
