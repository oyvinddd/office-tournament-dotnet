using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace office_tournament_api.ErrorHandling
{
    public static class ResultExtensions
    {
        public static ProblemDetails ToProblemDetails(Result result)
        {
            ProblemDetails problemDetails = new ProblemDetails();
            problemDetails.Status = GetStatusCode(result.Errors.FirstOrDefault().ErrorType);
            problemDetails.Title = GetTitle(result.Errors.FirstOrDefault().ErrorType);
            problemDetails.Type = GetType(result.Errors.FirstOrDefault().ErrorType);
            problemDetails.Extensions = new Dictionary<string, object?>()
                {
                    { "errors", new[] { result.Errors }}
                };

            return problemDetails;
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
