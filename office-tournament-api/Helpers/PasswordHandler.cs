using Microsoft.AspNetCore.Identity;

namespace office_tournament_api.Helpers
{
    public class PasswordHandler
    {
        public PasswordHandler() { }

        /// <summary>
        /// Creates a hashed version of a password 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string CreatePassword(string password)
        {
            string hashedPassword = new PasswordHasher<object>().HashPassword(null, password);

            return hashedPassword;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            bool isValid = true;
            var passwordVerificationResult = new PasswordHasher<object?>().VerifyHashedPassword(null, hashedPassword, password);
            switch (passwordVerificationResult)
            {
                case PasswordVerificationResult.Failed:
                        isValid = false;
                        break;

                case PasswordVerificationResult.Success:
                        isValid = true;
                    break;
            }

            return isValid;
        }
    }
}
