using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;

namespace office_tournament_api.Services
{
    public class MatchService
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
        public async Task<TournamentResult> CreateMatch(HttpContext httpContext, DTOMatchRequest dtoMatch)
        {
            TournamentResult result = new TournamentResult(true, new List<string>(), "");
            Guid? accountId = TokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                string error = "There was an error parsing AccountId from token";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            Tournament? tournament = await _context.Tournaments.FindAsync(dtoMatch.TournamentId);

            if (tournament == null)
            {
                string error = $"Tournament with id = {dtoMatch.TournamentId} was not found";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            Account? account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                string error = $"Account with id = {accountId} was not found";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            Account? opponentAccount = await _context.Accounts.FindAsync(dtoMatch.OpponentId);

            if (account == null)
            {
                string error = $"Opponent Account with id = {dtoMatch.OpponentId} was not found";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            if (!result.IsValid)
                return result;

            EloRatingResult eloRatingResult = _eloRating.CalculateEloRating(account.Score, opponentAccount.Score, true);

            try
            {
                Match match = new Match();
                match.Tournament = tournament;
                match.Winner = account;
                match.Loser = opponentAccount;
                match.WinnerDeltaScore = eloRatingResult.WinnerPointsWon;
                match.LoserDeltaScore = eloRatingResult.LoserPointsLost;
                match.Date = DateTime.UtcNow;

                account.Score = eloRatingResult.PlayerANewRating;
                opponentAccount.Score = eloRatingResult.PlayerBNewRating;

                await _context.Matches.AddAsync(match);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                string error = $"Insert of Match failed during save to database. Message: {e.Message}. InnerException: {e.InnerException}";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            return result;
        }
    }
}
