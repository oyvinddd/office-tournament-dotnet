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

        /// <summary>
        /// Gets a Tournament by a query of its Title
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("search/{query}")]
        [ProducesResponseType(typeof(List<DTOTournamentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Gets a Tournament by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DTOAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Gets the Admin (TournamentAccount) of a given Tournament
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <returns></returns>
        [HttpGet("admin")]
        [ProducesResponseType(typeof(DTOTournamentAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TournamentAccount?>> GetAdmin(Guid tournamentId)
        {
            try
            {
                TournamentAccount? admin = await _tournamentService.GetAdmin(tournamentId);

                if (admin == null)
                {
                    string error = $"No admin was found for this tournament";
                    return NotFound(error);
                }
                DTOTournamentAccountResponse dtoAdmin = _mapper.TournamentAccountDbToDto(admin);
                return Ok(dtoAdmin);
            }
            catch (Exception ex)
            {
                string error = $"JoinTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }

        /// <summary>
        /// Gets the current active tournament for the logged in account
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(DTOTournamentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
        /// Removes connection of a TournamentAccount to a Tournament, essentially leaving that Tournament
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="joinInfo"></param>
        /// <returns></returns>
        [HttpPut("leave/{tournamentId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateTournament(DTOTournamentRequest dtoTournament)
        {
            try
            {
                TournamentResult tournamentResult = await _tournamentService.CreateTournament(HttpContext, dtoTournament);

                if(!tournamentResult.IsValid)
                    return BadRequest(tournamentResult.Errors);

                return Created("CreateTournament", tournamentResult.Tournament.Code);
            }
            catch (Exception ex)
            {
                string error = $"CreateTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
