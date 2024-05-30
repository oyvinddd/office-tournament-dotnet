using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;
using office_tournament_api.Validators;
using System.Net.Http;

namespace office_tournament_api.Controllers
{
    [Route("tournaments")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITournamentService _tournamentService;
        public TournamentController(DataContext context, ITournamentService tournamentService)
        {
            _context = context;
            _tournamentService = tournamentService;
        }

        [HttpGet("{id}")]
        public async Task<Tournament>

        /// <summary>
        /// Add an Account to a Tournament
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="joinInfo"></param>
        /// <returns></returns>
        [HttpPut("join/{tournamentId}")]
        public async Task<ActionResult> JoinTournament(Guid tournamentId, DTOAccountJoinRequest joinInfo)
        {
            try
            {
                TournamentResult tournamentResult = await _tournamentService.JoinTournament(HttpContext, tournamentId, joinInfo);

                if (tournamentResult == null)
                    return BadRequest(tournamentResult.Errors);

                return Ok(tournamentResult.SucessMessage);
            }
            catch (Exception ex)
            {
                string error = $"JoinTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }

        /// <summary>
        /// Creates a new Tournament
        /// </summary>
        /// <param name="dtoTournament"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateTournament(DTOTournamentRequest dtoTournament)
        {
            try
            {
                TournamentResult tournamentResult = await _tournamentService.CreateTournament(dtoTournament);

                if(!tournamentResult.IsValid)
                    return BadRequest(tournamentResult.Errors);

                string response = "A new Tournament was created";
                return Created("CreateTournament", response);
            }
            catch (Exception ex)
            {
                string error = $"GetAccount failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
