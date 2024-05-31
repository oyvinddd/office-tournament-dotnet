using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public Guid? AdminId { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        public int ResetInterval { get; set; }
        [MaxLength(6)]
        public string Code { get; set; }
        public IList<Account> Participants { get; set; }
        public IList<Match> Matches { get; set; }
    }
}
