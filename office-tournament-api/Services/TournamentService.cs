using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
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
        private readonly DTOMapper _mapper;
        public TournamentService(DataContext context, DTOMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<DTOTournamentResponse>> SearchTournaments(string query)
        {
            List<Tournament> tournaments = await _context.Tournaments
                .Where(x => x.Title.Contains(query))
                .ToListAsync();

            List<DTOTournamentResponse> dtoTournaments = _mapper.ListTournamentDbToDto(tournaments);
            return dtoTournaments;
        }

        public async Task<Tournament?> GetTournament(Guid id)
        {
            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants.OrderByDescending(x => x.Score))
                    .ThenInclude(x => x.Account)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            return tournament;
        }

        public async Task<ValidationResult> JoinTournament(HttpContext httpContext, Guid tournamentId, DTOAccountJoinRequest joinInfo)
        {
            ValidationResult tournamentResult = new ValidationResult(true, new List<string>(), "");
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

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

            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.TournamentId == tournamentId && x.AccountId == accountId))
                .FirstOrDefaultAsync();

            if (account == null)
            {
                string error = $"Account with id = {accountId} was not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            TournamentAccount? tournamentAccount = account.TournamentAccounts.FirstOrDefault();

            if (tournamentAccount == null)
            {
                string error = $"TournamentAccount not found";
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
                tournamentAccount.Tournament = tournament;
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

        public async Task<ValidationResult> LeaveTournament(HttpContext httpContext, Guid tournamentId)
        {
            ValidationResult tournamentResult = new ValidationResult(true, new List<string>(), "");
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

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

            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.TournamentId == tournamentId && x.AccountId == accountId))
                .FirstOrDefaultAsync();

            if (account == null)
            {
                string error = $"Account with id = {accountId} was not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            TournamentAccount? tournamentAccount = account.TournamentAccounts.FirstOrDefault();

            if (tournamentAccount == null)
            {
                string error = $"TournamentAccount not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            if (!tournamentResult.IsValid)
                return tournamentResult;

            try
            {
                tournamentAccount.Tournament = null;
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

        /// <summary>
        /// Creates a new Tournament and TournamentAccounts for all active Tournaments 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="dtoTournament"></param>
        /// <returns></returns>
        public async Task<ValidationResult> ResetTournaments()
        {
            ValidationResult tournamentResult = new ValidationResult(true, new List<string>(), "");
            List<Tournament> existingTournaments = await _context.Tournaments
                .Include(x => x.Participants)
                    .ThenInclude(x => x.Account)
                .Where(x => x.IsActive)
                .ToListAsync();

            if(existingTournaments.IsNullOrEmpty())
            {
                string error = "No existing, active Tournaments were found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
                return tournamentResult;
            }

            try
            {

                List<TournamentAccount> newTournamentAccounts = [];
                Tournament newTournament = new Tournament();

                foreach (var existingTournament in existingTournaments)
                {
                    existingTournament.IsActive = false;

                    newTournament.AdminId = existingTournament.AdminId;
                    newTournament.Title = existingTournament.Title;
                    newTournament.ResetInterval = existingTournament.ResetInterval;
                    newTournament.Code = existingTournament.Code;
                    newTournament.IsActive = true;

                    TournamentAccount adminTourneyAccount = new TournamentAccount();
                    adminTourneyAccount.Tournament = existingTournament;
                    adminTourneyAccount.AccountId = (Guid)existingTournament.AdminId;
                    adminTourneyAccount.Score = 1600;
                    adminTourneyAccount.MatchesWon = 0;
                    adminTourneyAccount.MatchesPlayed = 0;

                    newTournamentAccounts.Add(adminTourneyAccount);

                    foreach (TournamentAccount tournamentAccount in existingTournament.Participants)
                    {
                        if (tournamentAccount.Id != newTournament.AdminId)
                        {
                            TournamentAccount newTournamentAccount = new TournamentAccount();
                            newTournamentAccount.Tournament = newTournament;
                            newTournamentAccount.AccountId = (Guid)tournamentAccount.AccountId;
                            newTournamentAccount.Score = 1600;
                            newTournamentAccount.MatchesWon = 0;
                            newTournamentAccount.MatchesPlayed = 0;

                            newTournamentAccounts.Add(newTournamentAccount);
                        }
                    }
                }

                await _context.Tournaments.AddAsync(newTournament);
                await _context.TournamentAccounts.AddRangeAsync(newTournamentAccounts);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                string error = $"Reset of Tournaments failed during save. Message: {e.Message}. InnerException: {e.InnerException}";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
            }

            string success = "All Tournaments were successfully reset";
            tournamentResult.SucessMessage = success;
            return tournamentResult;
        } 

        public async Task<ValidationResult> CreateTournament(HttpContext httpContext, DTOTournamentRequest dtoTournament)
        {
            ValidationResult tournamentResult = new ValidationResult(true, new List<string>(), "");
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);
            CodeBuilder codeBuilder = new CodeBuilder();
            Account? account = await _context.Accounts.FindAsync(accountId);

            if(account == null)
            {
                string error = $"Account not found";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
                return tournamentResult;
            }

            //Check if the Account already is admin for another tournament
            bool anyTournament = await _context.Tournaments
                .Where(x => x.AdminId == accountId && x.IsActive)
                .AnyAsync();

            if(anyTournament)
            {
                string error = "This account is already admin for an active tournament";
                tournamentResult.IsValid = false;
                tournamentResult.Errors.Add(error);
                return tournamentResult;
            }

            try
            {
                Tournament tournament = new Tournament();
                tournament.AdminId = accountId;
                tournament.Title = dtoTournament.Title;
                tournament.ResetInterval = dtoTournament.ResetInterval;
                tournament.Code = codeBuilder.RandomPassword();
                tournament.IsActive = true;

                TournamentAccount adminTourneyAccount = new TournamentAccount();
                adminTourneyAccount.Tournament = tournament;
                adminTourneyAccount.AccountId = (Guid)accountId;
                adminTourneyAccount.Score = 1600;
                adminTourneyAccount.MatchesWon = 0;
                adminTourneyAccount.MatchesPlayed = 0;

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
