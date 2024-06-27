using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace office_tournament_api.DTOs
{
    public class DTOPushTokenRequest : IValidatableObject
    {
        [JsonRequired]
        public string Token { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(Token))
            {
                yield return new ValidationResult("Token is empty", new[] { nameof(Token) });
            }
        }
    }
}
