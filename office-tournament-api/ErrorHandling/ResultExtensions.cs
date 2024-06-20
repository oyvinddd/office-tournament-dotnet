using System.Runtime.CompilerServices;

namespace office_tournament_api.ErrorHandling
{
    public static class ResultExtensions
    {
        public static IResult ToProblemDetails(Result result)
        {
            return Results.Problem(
                statusCode: GetStatusCode(result.Error.ErrorType),
                title: GetTitle(result.Error.ErrorType),
                type: GetType(result.Error.ErrorType),
                extensions: new Dictionary<string, object?>
                {
                    { "errors", new[] { result.Error }}
                });
        }

        public static int GetStatusCode(ErrorType errorType)
        {
            if (errorType == ErrorType.NotFound)
            {
                return StatusCodes.Status404NotFound;
            }

            if (errorType == ErrorType.Validation)
            {
                return StatusCodes.Status400BadRequest;
            }

            if (errorType == ErrorType.Conflict)
            {
                return StatusCodes.Status409Conflict;
            }

            return StatusCodes.Status500InternalServerError;
        }

        public static string GetTitle(ErrorType errorType)
        {
            if (errorType == ErrorType.NotFound)
            {
                return "Not Found";
            }

            if (errorType == ErrorType.Validation)
            {
                return "Bad Request";
            }

            if (errorType == ErrorType.Conflict)
            {
                return "Conflict";
            }

            return "Server Failure";
        }

        public static string GetType(ErrorType errorType)
        {
            if (errorType == ErrorType.NotFound)
            {
                return "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4"; //Not Found
            }

            if (errorType == ErrorType.Validation)
            {
                return "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"; //Bad Request
            }

            if (errorType == ErrorType.Conflict)
            {
                return "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8"; //Conflict
            }

            return "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"; //Internal Server Error
        }
    }
}
