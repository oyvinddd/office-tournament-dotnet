using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public class MatchService : IMatchService
    {
        private readonly DataContext _context;
        private readonly IAccountValidator _accountValidator;
        private readonly EloRating _eloRating;
        public MatchService(DataContext context, IAccountValidator accountValidator, EloRating eloRating)
        {
            _context = context;
            _accountValidator = accountValidator;
            _eloRating = eloRating;
        }

        /// <summary>
        /// Create a new Match and calculates updated elo ratings etc for the Accounts involved. Assumes opponent is always loser
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="dtoMatch"></param>
        /// <returns></returns>
        public async Task<(Result, string?)> CreateMatch(HttpContext httpContext, DTOMatchRequest dtoMatch)
        {
            List<Error> errors = new List<Error>();
            TournamentResult result = new TournamentResult(true, new List<string>(), "");
            bool isValid = true;
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                string error = "There was an error parsing AccountId from token";
                errors.Add(ApplicationErrors.ParseError());
                isValid = false;
            }

            Tournament? tournament = await _context.Tournaments.FindAsync(dtoMatch.TournamentId);

            if (tournament == null)
            {
                string error = $"Tournament with id = {dtoMatch.TournamentId} was not found";
                errors.Add(TournamentErrors.NotFound(dtoMatch.TournamentId));
                isValid = false;
            }

            if (!isValid)
                return (Result.Failure(errors), null);

            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.AccountId == accountId && x.TournamentId == dtoMatch.TournamentId))
                .FirstOrDefaultAsync();

            if (account == null)
            {
                string error = $"Account with id = {accountId} was not found";
                errors.Add(AccountErrors.NotFound((Guid)accountId));
                return (Result.Failure(errors), null);
            }

            TournamentAccount? tournamentAccountUser = account.TournamentAccounts.FirstOrDefault();

            if(tournamentAccountUser == null)
            {
                errors.Add(TournamentAccountErrors.NotFoundByTournamentAndAccount(dtoMatch.TournamentId, (Guid)accountId));
                return (Result.Failure(errors), null);
            }   

            Account? opponentAccount = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.AccountId == dtoMatch.OpponentId && x.TournamentId == dtoMatch.TournamentId))
                .FirstOrDefaultAsync();

            if (opponentAccount == null)
            {
                errors.Add(MatchErrors.NotFoundOpponentAccount(dtoMatch.OpponentId));
                return (Result.Failure(errors), null);
            }

            TournamentAccount? oppTournamentAccount = account.TournamentAccounts.FirstOrDefault();

            if (oppTournamentAccount == null)
            {
                errors.Add(MatchErrors.NotFoundOpponentTournamentAccount(dtoMatch.OpponentId, dtoMatch.TournamentId));
                return (Result.Failure(errors), null);
            }

            EloRatingResult eloRatingResult = _eloRating.CalculateEloRating(tournamentAccountUser.Score, oppTournamentAccount.Score, true);

            try
            {
                Match match = new Match();
                match.Tournament = tournament;
                match.Winner = tournamentAccountUser;
                match.Loser = oppTournamentAccount;
                match.WinnerDeltaScore = eloRatingResult.WinnerPointsWon;
                match.LoserDeltaScore = eloRatingResult.LoserPointsLost;
                match.Date = DateTime.UtcNow;

                tournamentAccountUser.Score = eloRatingResult.PlayerANewRating;
                tournamentAccountUser.MatchesWon += 1;
                tournamentAccountUser.MatchesPlayed += 1;
                account.TotalMatchesWon += 1;
                account.TotalMatchesPlayed += 1;

                oppTournamentAccount.Score = eloRatingResult.PlayerBNewRating;
                oppTournamentAccount.MatchesPlayed += 1;
                opponentAccount.TotalMatchesPlayed += 1;

                await _context.Matches.AddAsync(match);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                string error = $"Insert of Match failed during save to database. Message: {e.Message}. InnerException: {e.InnerException}";
                errors.Add(MatchErrors.DatabaseSaveFailure(error));
                return (Result.Failure(errors), null);
            }

            string message = "A new Match was created";
            return (Result.Success(), message);
        }
    }
}
