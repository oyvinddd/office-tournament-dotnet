using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    public class Tournament
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(AdminId))]
        public TournamentAccount Admin { get; set; }
        public Guid AdminId { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        public int ResetInterval { get; set; }
        [MaxLength(6)]
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public IList<TournamentAccount> Participants { get; set; }
        public IList<Match> Matches { get; set; }

        public Tournament() { }

        public Tournament(TournamentAccount admin, Guid adminId, string title, int resetInterval, string code, bool isActive)
        {
            Admin = admin;
            AdminId = adminId;
            Title = title;
            ResetInterval = resetInterval;
            Code = code;
            IsActive = isActive;
        }
    }
}
