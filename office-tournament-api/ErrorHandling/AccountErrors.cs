namespace office_tournament_api.ErrorHandling
{
    public class AccountErrors
    {
        public static Error NotFound(Guid id)
        {   
            return Error.NotFound("Accounts.NotFound", $"The Account with id '{id}' was not found");
        }

        public static Error UserNameNotFound(string username)
        {
            return Error.NotFound("Accounts.UserNameNotFound", $"The Account with username '{username}' was not found");
        }

        public static Error InvalidEmail()
        {
            Error newError = Error.Validation("Accounts.InvalidEmail", $"Email was invalid");

            return newError;
        }

        public static Error ExistingEmail()
        {
            Error newError = Error.Validation("Accounts.InvalidEmail", "An account with that email already exists");

            return newError;
        }

        public static Error ExistingUsername()
        {
            Error newError = Error.Validation("Accounts.ExistingUsername", "An account with the same username already exists");

            return newError;
        }

        public static Error DatabaseFail(string message)
        {
            Error newError = Error.Failure("Accounts.DatabaseFailure", $"Insert of Account failed during database save. Message: {message}");

            return newError;
        }

        public static Error InvalidPassword()
        {
            Error newError = Error.Validation("Accounts.InvalidPassword", "Password was incorrect");

            return newError;
        }
    }
}
