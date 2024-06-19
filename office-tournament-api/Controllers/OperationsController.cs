using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using office_tournament_api.office_tournament_db;

namespace office_tournament_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        public OperationsController(DataContext dataContext, IConfiguration configuration)
        {
            _context = dataContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Run migrations
        /// </summary>
        /// <returns></returns>
        [HttpGet("migrations")]
        public async Task<ActionResult> RunMigrations()
        {
            try
            {
                await _context.Database.MigrateAsync();
                return Ok("Migration run successfully");
            }
            catch (Exception e)
            {
                string error = $"RunMigrations failed. Message : {e.Message}. InnerException: {e.InnerException}";
                return BadRequest(error);
            }
        }

        //[HttpGet("connection-string")]
        //public async Task<ActionResult> GetConnectionString()
        //{
        //    try
        //    {
        //        string connectionString = _configuration.GetConnectionString("DefaultConnection");

        //        return Ok(connectionString);

        //    }
        //    catch (Exception e)
        //    {
        //        string error = $"GetConnectionString failed. Message : {e.Message}. InnerException: {e.InnerException}";
        //        return BadRequest(error);
        //    }
        //}


        /// <summary>
        /// Get all migrations that have run on the database
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-migrations")]
        public async Task<ActionResult<IEnumerable<string>>> GetMigrations()
        {
            try
            {
                IEnumerable<string> migrations = _context.Database.GetAppliedMigrations();

                return Ok(migrations);

            }
            catch (Exception e)
            {
                string error = $"GetMigrations failed. Message : {e.Message}. InnerException: {e.InnerException.ToString()}";
                return BadRequest(error);
            }
        }
    }
}
