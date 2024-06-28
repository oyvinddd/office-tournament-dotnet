using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using office_tournament_api.DTOs;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;
using office_tournament_api.Validators;

namespace office_tournament_api.Controllers
{
    [Route("tournament-accounts")]
    [ApiController]
    [Authorize]
    public class TournamentAccountController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly DTOMapper _mapper;
        public TournamentAccountController(DataContext context, ITournamentService tournamentService, DTOMapper dtoMapper)
        {
            _tournamentService = tournamentService;
            _mapper = dtoMapper;
        }

        /// <summary>
        /// Creates a new TournamentAccount
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

                return Created("CreateTournament", "");
            }
            catch (Exception ex)
            {
                string error = $"CreateTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
