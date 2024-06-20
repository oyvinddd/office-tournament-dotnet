using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly DTOMapper _mapper;
        public TournamentController(DataContext context, ITournamentService tournamentService, DTOMapper dtoMapper)
        {
            _tournamentService = tournamentService;
            _mapper = dtoMapper;
        }

        [HttpGet("search/{query}")]
        public async Task<ActionResult<List<DTOTournamentResponse>>> SearchTournaments(string query)
        {
            try
            {
                List<DTOTournamentResponse> dtoTournaments = await _tournamentService.SearchTournaments(query);

                return Ok(dtoTournaments);
            }
            catch (Exception ex)
            {
                string error = $"SearchTournaments failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTOTournamentResponse>> GetTournament(Guid id)
        {
            try
            {
                Tournament? tournament = await _tournamentService.GetTournament(id);

                if(tournament == null)
                {
                    string error = $"Tournament with id = {id} was not found";
                    return NotFound(error);
                }

                DTOTournamentResponse dtoTournament = _mapper.TournamentDbToDto(tournament);
                return Ok(dtoTournament);
            }
            catch (Exception ex)
            {
                string error = $"JoinTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }


        [HttpGet("active")]
        public async Task<ActionResult<DTOTournamentResponse>> GetActiveTournamentForAccount()
        {
            try
            {
                TournamentResult? tournamentResult = await _tournamentService.GetActiveTournamentForAccount(HttpContext);

                if (tournamentResult.Tournament == null)
                {
                    string error = $"No active Tournament was found for this account";
                    return NotFound(error);
                }

                DTOTournamentResponse dtoTournament = _mapper.TournamentDbToDto(tournamentResult.Tournament);
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
        /// Creates a new TournamentAccount for a Tournament and Account, and adds it to the Tournament
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
        /// Resets all Tournaments and creates new ones.
        /// </summary>
        /// <returns></returns>
        [HttpGet("reset")]
        public async Task<ActionResult<string>> ResetTournaments()
        {
            try
            {
                TournamentResult tournamentResult = await _tournamentService.ResetTournaments();

                if (!tournamentResult.IsValid)
                    return BadRequest(tournamentResult.Errors);

                return Created("ResetTournaments", tournamentResult.SucessMessage);
            }
            catch (Exception ex)
            {
                string error = $"ResetTournaments failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
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
                TournamentResult tournamentResult = await _tournamentService.CreateTournament(HttpContext, dtoTournament);

                if(!tournamentResult.IsValid)
                    return BadRequest(tournamentResult.Errors);

                return Created("CreateTournament", tournamentResult.SucessMessage);
            }
            catch (Exception ex)
            {
                string error = $"CreateTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
