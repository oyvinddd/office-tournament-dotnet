using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace office_tournament_api.DTOs
{
    public class DTOAccountRequest : IValidatableObject
    {
        public Guid? TournamentId { get; set; }
        public Guid? AdminTournamentId { get; set; }
        [MaxLength(100)]
        [JsonRequired]
        public string Email { get; set; }
        [MaxLength(50)]
        [JsonRequired]
        public string UserName { get; set; }
        [JsonRequired]
        public string Password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(Email))
            {
                yield return new ValidationResult("Email must have a value", new[] { nameof(Email) });
            }else
            {

                bool isValidEmail = true;

                try
                {
                    var email = new MailAddress(Email);
                }
                catch (Exception ex)
                {
                    isValidEmail = false;
                }

                if (!isValidEmail)
                {
                    yield return new ValidationResult("Email was invalid", new[] { nameof(Email) });
                }
            }

            if(string.IsNullOrEmpty(UserName))
            {
                yield return new ValidationResult("UserName must have a value", new[] { nameof(UserName) });
            }else
            {
                bool isValidUserName = IsValidUserName(UserName);

                if(!isValidUserName)
                {
                    yield return new ValidationResult("UserName was invalid. Username can only have small or large characters and numbers, and must be between 3 and 20 characters", 
                        new[] { nameof(UserName) });
                }
            }

            if(string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Password must have a value", new[] { nameof(Password) });
            }else
            {
                bool isValidPassword = IsValidPassword(Password); 

                if(!isValidPassword)
                {
                    yield return new ValidationResult("Password was invalid. Passwords need to be between 8 and 20 in length, and have one lower case letter, one upper case letter " +
                        "and one digit", new[] { nameof(Password) });
                }
            }
        }

        private bool IsValidUserName(string userName)
        {
            bool isValid = true;
            string regex = "^[a-zA-Z0-9]+$";

            bool isMatch = Regex.IsMatch(userName, regex);
            bool correctLength = userName.Length > 2 && userName.Length <= 20;

            if(!isMatch || !correctLength)
            {
                isValid = false;
            }

            return isValid;
        }

        private bool IsValidPassword(string password)
        {
            const int MIN_LENGTH = 8;
            const int MAX_LENGTH = 20;

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit;

            return isValid;
        }
    }
}
