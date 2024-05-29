using System.IdentityModel.Tokens.Jwt;

namespace office_tournament_api.Helpers
{

    public class TokenHandler
    {
        /// <summary>
        /// Find name of current user (set UpdatedBy))
        /// </summary>
        /// <returns></returns>
        public static Guid? GetIdFromToken(HttpContext httpContext)
        {
            Guid guid;
            var headers = httpContext.Request.Headers;

            if (headers["authorization"].Count == 0)
                return null;

            if (headers["Authorization"].Count == 0 || headers["Authorization"].First().Length < 7)
                return null;

            var clientToken = headers["Authorization"].First().Substring(7);

            // Assume 'token' is the JWT token string 
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(clientToken); // Retrieve the 'name' claim from the JWT token 
            var account = jwtToken.Claims.FirstOrDefault(claims => claims.Type == "sub");

            if (account == null)
                return null;

            string accountId = account.Value;

            try
            {
                guid = new Guid(accountId);
            }
            catch (Exception ex)
            {
                return null;
            }

            return guid;
        }
    }
}
