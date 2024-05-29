﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.DTOs;
using office_tournament_api.Helpers;
using office_tournament_api.office_tournament_db;
using System.Net.Http;

namespace office_tournament_api.Controllers
{
    [Route("tournaments")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly DataContext _context;
        public TournamentController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add an Account to a Tournament
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="joinInfo"></param>
        /// <returns></returns>
        [HttpPut("join/{tournamentId}")]
        public async Task<ActionResult> JoinTournament(Guid tournamentId, DTOAccountJoinRequest joinInfo)
        {
            try
            {
                Guid? accountId = TokenHandler.GetIdFromToken(HttpContext);

                if(accountId == null)
                {
                    string error = "There was an error parsing AccountId from token";
                    return BadRequest(error);
                }

                Tournament? tournament = await _context.Tournaments
                    .Include(x => x.Participants)
                    .Where(x => x.Id == tournamentId)
                    .FirstOrDefaultAsync();

                if (tournament == null)
                {
                    string error = $"Tournament with id = {tournamentId} was not found";
                    return NotFound(error);
                }

                Account? account = await _context.Accounts.FindAsync(accountId);

                if (account == null)
                {
                    string error = $"Account with id = {accountId} was not found";
                    return NotFound(error);
                }

                if (!tournament.Code.Equals(joinInfo.Code))
                {
                    string error = $"Code supplied doest not match code of Tournament with id = {tournamentId}";
                    return BadRequest(error);
                }

                account.Tournament = tournament;

                await _context.SaveChangesAsync();

                string response = $"Account with id = {accountId} joined Tournament with id = {tournamentId}";
                return Ok(response);
            }
            catch (Exception ex)
            {
                string error = $"JoinTournament failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }

        /// <summary>
        /// Creates a new Tournament
        /// </summary>
        /// <param name="dtoTournament"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateTournament(DTOTournamentRequest dtoTournament)
        {
            try
            {
                CodeBuilder codeBuilder = new CodeBuilder();

                Tournament tournament = new Tournament();
                tournament.Title = dtoTournament.Title;
                tournament.ResetInterval = dtoTournament.ResetInterval;
                tournament.Code = codeBuilder.RandomPassword();

                await _context.Tournaments.AddAsync(tournament);
                await _context.SaveChangesAsync();

                string response = "A new Tournament was created";
                return Created("CreateTournament", response);
            }
            catch (Exception ex)
            {
                string error = $"GetAccount failed. Message: {ex.Message}. InnerException: {ex.InnerException}";
                return StatusCode((int)StatusCodes.Status500InternalServerError, error);
            }
        }
    }
}
