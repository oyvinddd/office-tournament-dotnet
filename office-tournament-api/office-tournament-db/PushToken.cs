using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    public class PushToken
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
        public string Token { get; set; }
    }
}
