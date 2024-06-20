namespace office_tournament_api.ErrorHandling
{
    public class AccountErrors
    {
        public static Error NotFound(Guid id)
        {   
            return Error.NotFound("Accounts.NotFound", $"The Account with id '{id}' was not found"); ;
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
    }
}
