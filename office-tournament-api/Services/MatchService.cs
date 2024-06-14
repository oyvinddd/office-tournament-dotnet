using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
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
        public async Task<ValidationResult> CreateMatch(HttpContext httpContext, DTOMatchRequest dtoMatch)
        {
            ValidationResult result = new ValidationResult(true, new List<string>(), "");
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

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

            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.AccountId == accountId && x.TournamentId == dtoMatch.TournamentId))
                .FirstOrDefaultAsync();

            if (account == null)
            {
                string error = $"Account with id = {accountId} was not found";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            TournamentAccount? tournamentAccountUser = account.TournamentAccounts.FirstOrDefault();

            if(tournamentAccountUser == null)
            {
                string error = $"TournamentAccount for Account with id = {accountId} and Tournament with id = {dtoMatch.TournamentId} was not found";
                result.IsValid = false;
                result.Errors.Add(error);
            }
            

            Account? opponentAccount = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.AccountId == dtoMatch.OpponentId && x.TournamentId == dtoMatch.TournamentId))
                .FirstOrDefaultAsync();

            if (account == null)
            {
                string error = $"Opponent Account with id = {dtoMatch.OpponentId} was not found";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            TournamentAccount? oppTournamentAccount = account.TournamentAccounts.FirstOrDefault();

            if (oppTournamentAccount == null)
            {
                string error = $"Opponent TournamentAccount for Account with id = {dtoMatch.OpponentId} and Tournament with id = {dtoMatch.TournamentId} was not found";
                result.IsValid = false;
                result.Errors.Add(error);
            }

            if (!result.IsValid)
                return result;

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
                result.IsValid = false;
                result.Errors.Add(error);
            }

            return result;
        }
    }
}
