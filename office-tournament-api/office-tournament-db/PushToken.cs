using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    [Index(nameof(Token), IsUnique = true)]
    public class PushToken
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; }
        public Guid AccountId { get; set; }
        public string Token { get; set; }

        public PushToken() { }

        public PushToken(Guid accountId, string token)
        {
            AccountId = accountId;  
            Token = token;
        }
    }
}
