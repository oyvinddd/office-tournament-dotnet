namespace office_tournament_api.ErrorHandling
{
    public class PushTokenErrors
    {
        public static Error DatabaseFail(string message)
        {
            Error newError = Error.Failure("PushTokens.DatabaseFailure", $"Insert of PushToken failed during database save. Message: {message}");

            return newError;
        }

        public static Error CreatePushTokenFailure(string message)
        {
            Error newError = Error.Failure("PushTokens.CreatePushTokenFailure", $"CreatePushToken method failed. Message: {message}");

            return newError;
        }

        public static Error DuplicateToken()
        {
            Error newError = Error.Validation("PushTokens.DuplicateToken", "An identical token exists for another PushToken");

            return newError;
        }
    }
}
