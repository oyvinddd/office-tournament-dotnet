using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using office_tournament_api.DTOs;
using office_tournament_api.ErrorHandling;
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

        public async Task<TournamentAccount?> GetAdmin(Guid tournamentId)
        {
            TournamentAccount? admin = await _context.TournamentAccounts.Where(x => x.AdminTournamentId == tournamentId).FirstOrDefaultAsync();
            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Admin)
                .Where(x => x.Admin.Id == admin.Id)
                .FirstOrDefaultAsync();

            return admin;
        }

        public async Task<(Result, Tournament?)> GetActiveTournamentForAccount(HttpContext httpContext)
        {
            List<Error> errors = new List<Error>(); 
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                errors.Add(ApplicationErrors.ParseError());
                return (Result.Failure(errors),null);
            }

            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants)
                    .ThenInclude(x => x.Account)
                .Where(x => x.IsActive)
                .FirstOrDefaultAsync();

            if(tournament == null)
            {
                errors.Add(TournamentErrors.NoActiveTournament());
                return (Result.Failure(errors), null);
            }

            TournamentAccount? tournamentAccount = tournament.Participants.Where(x => x.AccountId == accountId).FirstOrDefault();

            if(tournamentAccount == null)
            {
                errors.Add(TournamentErrors.NoActiveTournament());
                return (Result.Failure(errors), null);
            }

            return (Result.Success(), tournament);
        }

        public async Task<(Result, Tournament?)> GetTournament(Guid id)
        {
            List<Error> errors = new List<Error>();
            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants.OrderByDescending(x => x.Score))
                    .ThenInclude(x => x.Account)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            if(tournament == null)
            {
                errors.Add(TournamentErrors.NotFound(id));
                return (Result.Failure(errors), null);
            }

            return (Result.Success(), tournament);
        }

        public async Task<Result> JoinTournament(HttpContext httpContext, Guid tournamentId, DTOAccountJoinRequest joinInfo)
        {
            bool isValid = true;
            List<Error> errors = new List<Error>();
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                errors.Add(ApplicationErrors.ParseError());
                return Result.Success();
            }

            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants)
                .Where(x => x.Id == tournamentId)
                .FirstOrDefaultAsync();

            if (tournament == null)
            {
                errors.Add(TournamentErrors.NotFound(tournamentId));
                isValid = false;
            }

            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.TournamentId == tournamentId && x.AccountId == accountId))
                .Where(x => x.Id == accountId)
                .FirstOrDefaultAsync();

            if (account == null)
            {
                errors.Add(AccountErrors.NotFound((Guid)accountId));
                isValid = false;
            }

            if (!isValid)
                return Result.Failure(errors);

            TournamentAccount? tournamentAccount = account.TournamentAccounts
                .Where(x => x.TournamentId == tournamentId && x.AccountId == accountId).FirstOrDefault();

            if (tournamentAccount != null)
            {
                errors.Add(TournamentErrors.AlreadyJoined());
                isValid = false;
            }

            if (!tournament.Code.Equals(joinInfo.Code))
            {
                errors.Add(TournamentErrors.InvalidCode(tournamentId));
                isValid = false;
            }

            if (!isValid)
                return Result.Failure(errors);

            try
            {
                TournamentAccount newTournamentAccount = new TournamentAccount();
                newTournamentAccount.Tournament = tournament;
                newTournamentAccount.Account = account;
                newTournamentAccount.Score = 1600;
                newTournamentAccount.MatchesWon = 0;
                newTournamentAccount.MatchesPlayed = 0;
                newTournamentAccount.UpdatedDate = DateTime.UtcNow;

                await _context.TournamentAccounts.AddAsync(newTournamentAccount);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                errors.Add(TournamentAccountErrors.DatabaseSaveFailure(e.Message));
                return Result.Failure(errors);
            }

            return Result.Success();
        }

        public async Task<Result> LeaveTournament(HttpContext httpContext, Guid tournamentId)
        {
            bool isValid = false;
            List<Error> errors = new List<Error>();
            TournamentResult tournamentResult = new TournamentResult();
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);

            if (accountId == null)
            {
                errors.Add(ApplicationErrors.ParseError());
                return Result.Failure(errors);
            }

            Tournament? tournament = await _context.Tournaments
                .Include(x => x.Participants)
                .Where(x => x.Id == tournamentId)
                .FirstOrDefaultAsync();

            if (tournament == null)
            {
                errors.Add(TournamentErrors.NotFound(tournamentId));
                isValid = false;
            }

            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts.Where(x => x.TournamentId == tournamentId && x.AccountId == accountId))
                .FirstOrDefaultAsync();

            if (account == null)
            {
                errors.Add(AccountErrors.NotFound((Guid)accountId));
                isValid = false;
            }

            if (!isValid)
                return Result.Failure(errors);

            TournamentAccount? tournamentAccount = account.TournamentAccounts
                .Where(x => x.TournamentId == tournamentId && x.AccountId == accountId).FirstOrDefault();

            if (tournamentAccount == null)
            {
                errors.Add(TournamentAccountErrors.NotFoundByTournamentAndAccount(tournamentId, (Guid)accountId));
                isValid = false;
            }

            if (!isValid)
                return Result.Failure(errors);

            try
            {
                tournamentAccount.Tournament = null;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                errors.Add(TournamentAccountErrors.DatabaseSaveFailureLeaveTournament(tournamentId, (Guid)accountId, e.Message));
                return Result.Failure(errors);
            }

            return Result.Success();
        }

        /// <summary>
        /// Creates a new Tournament and TournamentAccounts for all active Tournaments 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="dtoTournament"></param>
        /// <returns></returns>
        public async Task<Result> ResetTournaments()
        {
            var errors = new List<Error>();
            TournamentResult tournamentResult = new TournamentResult(true, new List<string>(), "");
            List<Tournament> existingTournaments = await _context.Tournaments
                .Include(x => x.Participants)
                    .ThenInclude(x => x.Account)
                .Where(x => x.IsActive)
                .ToListAsync();

            if(existingTournaments.IsNullOrEmpty())
            {
                errors.Add(TournamentErrors.ExistingTournamentsNotFound());
                return Result.Failure(errors);
            }

            try
            {
                List<Tournament> newTournaments = [];

                foreach (var existingTournament in existingTournaments)
                {
                    existingTournament.IsActive = false;

                    List<TournamentAccount> newTournamentAccounts = [];

                    Tournament newTournament = new Tournament();
                    newTournament.Title = existingTournament.Title;
                    newTournament.ResetInterval = existingTournament.ResetInterval;
                    newTournament.Code = existingTournament.Code;
                    newTournament.IsActive = true;

                    TournamentAccount adminTourneyAccount = new TournamentAccount();
                    adminTourneyAccount.Tournament = newTournament;
                    adminTourneyAccount.AdminTournament = newTournament;
                    adminTourneyAccount.AccountId = (Guid)existingTournament.Admin.AccountId;
                    adminTourneyAccount.Score = 1600;
                    adminTourneyAccount.MatchesWon = 0;
                    adminTourneyAccount.MatchesPlayed = 0;

                    newTournament.Admin = adminTourneyAccount;

                    newTournamentAccounts.Add(adminTourneyAccount);

                    foreach (TournamentAccount tournamentAccount in existingTournament.Participants)
                    {
                        if (tournamentAccount.AccountId != adminTourneyAccount.AccountId)
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
                    newTournament.Participants = newTournamentAccounts;
                    newTournaments.Add(newTournament);
                }

                await _context.Tournaments.AddRangeAsync(newTournaments);
                await _context.SaveChangesAsync();

                foreach (var newTournament in newTournaments)
                {
                    newTournament.AdminId = newTournament.Admin.Id;
                }

                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                string error = $"Reset of Tournaments failed during save. Message: {e.Message}";
                errors.Add(TournamentErrors.DatabaseSaveFailure(error));
                return Result.Failure(errors);
            }

            return Result.Success();
        } 

        public async Task<(Result, Tournament?)> CreateTournament(HttpContext httpContext, DTOTournamentRequest dtoTournament)
        {
            TournamentResult tournamentResult = new TournamentResult(true, new List<string>(), "");
            var errors = new List<Error>();
            Guid? accountId = JwtTokenHandler.GetIdFromToken(httpContext);
            CodeBuilder codeBuilder = new CodeBuilder();


            Account? account = await _context.Accounts
                .Include(x => x.TournamentAccounts)
                    .ThenInclude(x => x.Tournament)
                .Where(x => x.Id == accountId)
                .FirstOrDefaultAsync();

            if(account == null)
            {
                errors.Add(AccountErrors.NotFound((Guid)accountId));
                return (Result.Failure(errors), null);
            }

            //Check if the Account already is admin for another tournament
            bool anyTournament = account.TournamentAccounts.Where(x => x.Tournament.AdminId == x.Id && x.Tournament.IsActive).Any();

            if(anyTournament)
            {
                errors.Add(TournamentErrors.DuplicateAdmin((Guid)accountId));
                return (Result.Failure(errors), null);
            }

            Tournament tournament = new Tournament();

            try
            {
                TournamentAccount adminTourneyAccount = new TournamentAccount();
                adminTourneyAccount.AccountId = (Guid)accountId;
                adminTourneyAccount.Score = 1600;
                adminTourneyAccount.MatchesWon = 0;
                adminTourneyAccount.MatchesPlayed = 0;

                tournament.Admin = adminTourneyAccount;
                tournament.Title = dtoTournament.Title;
                tournament.ResetInterval = dtoTournament.ResetInterval;
                tournament.Code = codeBuilder.RandomPassword();
                tournament.IsActive = true;
                tournament.Participants = new List<TournamentAccount> { adminTourneyAccount };
   
                await _context.Tournaments.AddAsync(tournament);
                await _context.SaveChangesAsync();

                tournament.AdminId = adminTourneyAccount.Id;
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                string error = $"Insert of Tournament failed during database save. Message: {e.Message}";
                errors.Add(TournamentErrors.DatabaseSaveFailure(error));
                return (Result.Failure(errors), null);
            }

            return (Result.Success(), tournament);
        }
    }
}
