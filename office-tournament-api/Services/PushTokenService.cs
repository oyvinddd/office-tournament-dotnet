using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public class PushTokenService : IPushTokenService
    {
        private readonly DataContext _context;
        private readonly PasswordHandler _passwordHandler;
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly DTOMapper _mapper;
        public PushTokenService(DataContext context, PasswordHandler passwordHandler, JwtTokenHandler jwtTokenHandler, DTOMapper mapper)
        {
            _context = context;
            _passwordHandler = passwordHandler;
            _jwtTokenHandler = jwtTokenHandler;
            _mapper = mapper;
        }

        public async Task<Result> CreatePushToken(HttpContext httpContext, DTOPushTokenRequest dtoToken)
        {
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                return Result.Failure(new List<Error> { ApplicationErrors.ParseError() });
            }

            Account? account = await _context.Accounts.FindAsync(accountId);

            if(account == null)
            {
                return Result.Failure(new List<Error> { AccountErrors.NotFound((Guid)accountId) });
            }

            bool duplicateToken = await _context.PushTokens.Where(x => x.Token.Equals(dtoToken.Token)).AnyAsync();

            if(duplicateToken)
            {
                return Result.Failure(new List<Error> { PushTokenErrors.DuplicateToken() });
            }

            try
            {
                PushToken pushToken = new PushToken(account.Id, dtoToken.Token);
                await _context.PushTokens.AddAsync(pushToken);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Result.Failure(new List<Error> { PushTokenErrors.DatabaseFail(e.Message) });
            }

            return Result.Success();
        }
    }
}
