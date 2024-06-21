using Microsoft.IdentityModel.Tokens;

namespace office_tournament_api.ErrorHandling
{
    public class Result
    {
        public List<Error> Errors { get; }
        public bool IsSuccess { get; }
        private Result(bool isSuccess, List<Error> errors)
        {

            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsFailure => !IsSuccess;

        public static Result Success() => new(true, new List<Error> { Error.None() });

        public static Result Failure(List<Error> errors) => new(false, errors);
    }
}
