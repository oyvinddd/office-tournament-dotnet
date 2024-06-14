using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace office_tournament_api.office_tournament_db
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Account
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public int TotalMatchesWon { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public IList<TournamentAccount> TournamentAccounts { get; set;}
    }
}
