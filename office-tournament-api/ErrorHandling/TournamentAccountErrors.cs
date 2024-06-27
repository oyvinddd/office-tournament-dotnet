namespace office_tournament_api.ErrorHandling
{
    public class TournamentAccountErrors
    {
        public static Error NotFound(Guid id)
        {
            return Error.NotFound("TournamentAccounts.NotFound", $"The Account with id '{id}' was not found");
        }

        public static Error UserNameNotFound(string username)
        {
            return Error.NotFound("TournamentAccounts.UserNameNotFound", $"The Account with username '{username}' was not found");
        }

        public static Error InvalidLoginDetails()
        {
            return Error.Validation("TournamentAccounts.InvalidLoginDetails", "Invalid login details");
        }

        public static Error InvalidEmail()
        {
            Error newError = Error.Validation("TournamentAccounts.InvalidEmail", $"Email was invalid");

            return newError;
        }

        public static Error ExistingEmail()
        {
            Error newError = Error.Validation("TournamentAccounts.ExistingEmail", "An account with that email already exists");

            return newError;
        }

        public static Error ExistingUsername()
        {
            Error newError = Error.Validation("TournamentAccounts.ExistingUsername", "An account with the same username already exists");

            return newError;
        }

        public static Error DatabaseFail(string message)
        {
            Error newError = Error.Failure("TournamentAccounts.DatabaseFailure", $"Insert of Account failed during database save. Message: {message}");

            return newError;
        }

        public static Error InvalidPassword()
        {
            Error newError = Error.Validation("TournamentAccounts.InvalidPassword", "Password was incorrect");

            return newError;
        }
    }
}
