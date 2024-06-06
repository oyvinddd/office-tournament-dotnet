using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using office_tournament_api.Middleware;
using office_tournament_api.office_tournament_db;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace office_tournament_api.Helpers
{

    public class JwtTokenHandler
    {
        private readonly JwtOptions _jwtOptions;
        public JwtTokenHandler(IOptions<JwtOptions> jwtOptions) 
        {
            _jwtOptions = jwtOptions.Value;
        }


        public string GetJwtToken(HttpContext httpContext)
        {
            var headers = httpContext.Request.Headers;

            if (headers["authorization"].Count == 0)
                return null;

            if (headers["Authorization"].Count == 0 || headers["Authorization"].First().Length < 7)
                return null;

            var clientToken = headers["Authorization"].First().Substring(7);
            
            return clientToken;
        }
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

        public string CreateToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: _jwtOptions.Issuer,
              audience: _jwtOptions.Audience,
              claims: new[] {
                    new Claim("sub", account.Id.ToString())
              },
              expires: DateTime.UtcNow.AddHours(1),
              signingCredentials: creds);

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        // Validate a JWT token
        //public ClaimsPrincipal ValidateJwtToken(string token)
        //{
        //    // Retrieve the JWT secret from environment variables and encode it
        //    var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!);

        //    try
        //    {
        //        // Create a token handler and validate the token
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        //            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        //            IssuerSigningKey = new SymmetricSecurityKey(key)
        //        }, out SecurityToken validatedToken);

        //        // Return the claims principal
        //        return claimsPrincipal;
        //    }
        //    catch (SecurityTokenExpiredException)
        //    {
        //        // Handle token expiration
        //        throw new ApplicationException("Token has expired.");
        //    }
        //}
    }
}
