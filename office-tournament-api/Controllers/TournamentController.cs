using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;
using office_tournament_api.Validators;
using System.Net.Http;
using System.Runtime;

namespace office_tournament_api.Controllers
{
    [Route("tournaments")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly DTOMapper _dtoMapper;
        public TournamentController(DataContext context, ITournamentService tournamentService, DTOMapper dtoMapper)
        {
            _tournamentService = tournamentService;
            _dtoMapper = dtoMapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tournament>> GetTournament(Guid id)
        {
            try
            {
                Tournament? tournament = await _tournamentService.GetTournament(id);

                if(tournament == null)
                {
                    string error = $"Tournament with id = {id} was not found";
                    return NotFound(error);
                }

                DTOTournamentResponse dtoTournament = _dtoMapper.TournamentDbToDto(tournament);
                return Ok(dtoTournament);
            }
            catch (Exception ex)
            {
                string error = $"JoinTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }

        /// <summary>
        /// Add an Account to a Tournament
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="joinInfo"></param>
        /// <returns></returns>
        [HttpPut("leave/{tournamentId}")]
        public async Task<ActionResult> LeaveTournament(Guid tournamentId)
        {
            try
            {
                TournamentResult tournamentResult = await _tournamentService.LeaveTournament(HttpContext, tournamentId);

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

                return Created("CreateTournament", tournamentResult.SucessMessage);
            }
            catch (Exception ex)
            {
                string error = $"GetAccount failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
