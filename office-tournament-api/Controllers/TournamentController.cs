using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DTOTournamentResponse>>> SearchTournaments(string query)
        {
            try
            {
                List<DTOTournamentResponse> dtoTournaments = await _tournamentService.SearchTournaments(query);

                return Ok(dtoTournaments);
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error> 
                    { TournamentErrors.SearchTournamentError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOTournamentResponse>> GetTournament(Guid id)
        {
            try
            {
                (Result result, Tournament? tournament) = await _tournamentService.GetTournament(id);

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                DTOTournamentResponse dtoTournament = _mapper.TournamentDbToDto(tournament);
                return Ok(dtoTournament);
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error> 
                    { TournamentErrors.GetTournamentError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// Gets the Admin (TournamentAccount) of a given Tournament
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <returns></returns>
        [HttpGet("admin")]
        [ProducesResponseType(typeof(DTOTournamentAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<TournamentAccount?>> GetAdmin(Guid tournamentId)
        {
            try
            {
                (Result result, TournamentAccount? admin) = await _tournamentService.GetAdmin(tournamentId);

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                DTOTournamentAccountResponse dtoAdmin = _mapper.TournamentAccountDbToDto(admin);
                return Ok(dtoAdmin);
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error>
                    { TournamentErrors.GetAdminError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// Gets the current active tournament for the logged in account
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        [ProducesResponseType(typeof(DTOTournamentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DTOTournamentResponse>> GetActiveTournamentForAccount()
        {
            try
            {
                (Result result, Tournament tournament) = await _tournamentService.GetActiveTournamentForAccount(HttpContext);

                if(result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                DTOTournamentResponse dtoTournament = _mapper.TournamentDbToDto(tournament);
                return Ok(dtoTournament);
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error>
                    { TournamentErrors.GetActiveTournamentForAccountError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> LeaveTournament(Guid tournamentId)
        {
            try
            {
                Result result = await _tournamentService.LeaveTournament(HttpContext, tournamentId);

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error>
                    { TournamentErrors.LeaveTournamentError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
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
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> JoinTournament(Guid tournamentId, DTOAccountJoinRequest joinInfo)
        {
            try
            {
                Result result = await _tournamentService.JoinTournament(HttpContext, tournamentId, joinInfo);

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error>
                    { TournamentErrors.JoinTournamentError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
            }
        }


        /// <summary>
        /// Resets all Tournaments and creates new ones.
        /// </summary>
        /// <returns></returns>
        [HttpGet("reset")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> ResetTournaments()
        {
            try
            {
                Result result = await _tournamentService.ResetTournaments();

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error>
                    { TournamentErrors.ResetTournamentsError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
            }
        }

        /// <summary>
        /// Creates a new Tournament and returns the code of the Tournament
        /// </summary>
        /// <param name="dtoTournament"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> CreateTournament(DTOTournamentRequest dtoTournament)
        {
            try
            {
                (Result result, Tournament? tournament) = await _tournamentService.CreateTournament(HttpContext, dtoTournament);

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }

                return Created("CreateTournament", tournament.Code);
            }
            catch (Exception ex)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error>
                    { TournamentErrors.GetAdminError(ex.Message, ex.InnerException.ToString()) }));
                return StatusCode((int)StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }
}
