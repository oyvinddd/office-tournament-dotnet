using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
using office_tournament_api.office_tournament_db;

namespace office_tournament_api.Services
{
    public class TournamentAccountService : ITournamentAccountService
    {
        private readonly DataContext _context;
        private readonly DTOMapper _mapper;
        public TournamentAccountService(DataContext context, DTOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result> CreateTournamentAccount(DTOTournamentAccountRequest dtoTournamentAccount)
        {
            Tournament? tournament = await _context.Tournaments.FindAsync(dtoTournamentAccount.TournamentId);
            Account? account = await _context.Accounts.FindAsync(dtoTournamentAccount.AccountId);

            return Result.Success();
        }
    }
}
