
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using office_tournament_api.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace office_tournament_api.Middleware
{
    public class JwtMiddleware 
    {
        private readonly JwtTokenHandler _jwtTokenHandler;

        public JwtMiddleware(JwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenHandler = jwtTokenHandler;
        }

        //public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        //{
        //    // Get the token from the Authorization header
        //    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        //    if (!token.IsNullOrEmpty())
        //    {

        //        try
        //        {
        //            // Verify the token using the JwtSecurityTokenHandlerWrapper
        //            //var claimsPrincipal = _jwtTokenHandler.ValidateJwtToken(token);

        //            // Extract the user ID from the token
        //            //var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //            // Store the user ID in the HttpContext items for later use
        //            context.Items["UserId"] = userId;

        //            // You can also do the for same other key which you have in JWT token.
        //        }
        //        catch (Exception e)
        //        {
        //            // If the token is invalid, throw an exception
        //        }


        //    }
        //    // Continue processing the request
        //    await next(context);

        //    throw new NotImplementedException();
        //}
    }
}
