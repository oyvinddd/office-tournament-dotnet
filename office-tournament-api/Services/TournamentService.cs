using Azure;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using office_tournament_api.Validators;
using System.Runtime;

namespace office_tournament_api.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly DataContext _context;
        public TournamentService(DataContext context)
        {
            _context = context;
        }

        public async Task<Tournament?> GetTournament(Guid id)
        {
            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants.OrderByDescending(x => x.Score))
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return tournament;
        }

        public async Task<TournamentResult> JoinTournament(HttpContext httpContext, Guid tournamentId, DTOAccountJoinRequest joinInfo)
        {
            TournamentResult tournamentResult = new TournamentResult(true, new List<string>(), "");
            Guid? accountId = TokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                string error = "There was an error parsing AccountId from token";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants)
                .Where(x => x.Id == tournamentId)
                .FirstOrDefaultAsync();

            if (tournament == null)
            {
                string error = $"Tournament with id = {tournamentId} was not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            Account? account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                string error = $"Account with id = {accountId} was not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            if (!tournament.Code.Equals(joinInfo.Code))
            {
                string error = $"Code supplied doest not match code of Tournament with id = {tournamentId}";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            if (!tournamentResult.IsValid)
                return tournamentResult;

            try
            {
                account.Tournament = tournament;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                string error = $"There was an error when trying to add Account with id = {accountId} to Tournament with id = {tournamentId}. Message: {e.Message}. " +
                    $"InnerException: {e.InnerException}";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            string successMessage = $"Account with id = {accountId} joined Tournament with id = {tournamentId}";
            tournamentResult.SucessMessage = successMessage;
            return tournamentResult;
        }

        public async Task<TournamentResult> LeaveTournament(HttpContext httpContext, Guid tournamentId)
        {
            TournamentResult tournamentResult = new TournamentResult(true, new List<string>(), "");
            Guid? accountId = TokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                string error = "There was an error parsing AccountId from token";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants)
                .Where(x => x.Id == tournamentId)
                .FirstOrDefaultAsync();

            if (tournament == null)
            {
                string error = $"Tournament with id = {tournamentId} was not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            Account? account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                string error = $"Account with id = {accountId} was not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            if (!tournamentResult.IsValid)
                return tournamentResult;

            try
            {
                account.Tournament = null;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                string error = $"There was an error when trying to remove Account with id = {accountId} from Tournament with id = {tournamentId}. Message: {e.Message}. " +
                    $"InnerException: {e.InnerException}";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            string successMessage = $"Account with id = {accountId} left Tournament with id = {tournamentId}";
            tournamentResult.SucessMessage = successMessage;

            return tournamentResult;
        } 

        public async Task<TournamentResult> CreateTournament(DTOTournamentRequest dtoTournament)
        {
            CodeBuilder codeBuilder = new CodeBuilder();
            var tournamentResult = new TournamentResult(true, new List<string>(), "");

            try
            {
                Tournament tournament = new Tournament();
                tournament.Title = dtoTournament.Title;
                tournament.ResetInterval = dtoTournament.ResetInterval;
                tournament.Code = codeBuilder.RandomPassword();

                await _context.Tournaments.AddAsync(tournament);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                string error = $"Insert of Tournament failed during save. Message: {e.Message}. InnerException: {e.InnerException}";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            string success = "A new Tournament was created";
            tournamentResult.SucessMessage = success;
            return tournamentResult;
        }
    }
}
