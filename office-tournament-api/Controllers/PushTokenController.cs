using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Services;
using System.Security.Principal;

namespace office_tournament_api.Controllers
{
    [Route("api/push-tokens")]
    [ApiController]
    [Authorize]
    public class PushTokenController : ControllerBase
    {
        private readonly IPushTokenService _pushTokenService;
        private readonly DTOMapper _mapper;
        public PushTokenController(IPushTokenService pushTokenService, DTOMapper dtoMapper)
        {
            _pushTokenService = pushTokenService;
            _mapper = dtoMapper;
        }

        /// <summary>
        /// Creates a new PushToken with the Account from token (current logged in Account)
        /// </summary>
        /// <param name="dtoToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Result>> CreatePushToken(DTOPushTokenRequest dtoToken)
        {
            try
            {
                Result result = await _pushTokenService.CreatePushToken(HttpContext, dtoToken);

                if (result.IsFailure)
                {
                    ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(result);
                    return StatusCode((int)problemDetails.Status, problemDetails);
                }
                return Created("CreateAccount", true);
            }
            catch (Exception e)
            {
                ProblemDetails problemDetails = ResultExtensions.ToProblemDetails(Result.Failure(new List<Error> { PushTokenErrors.CreatePushTokenFailure(e.Message) }));
                return StatusCode((int)problemDetails.Status, problemDetails);
            }
        } 
    }
}
