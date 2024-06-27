namespace office_tournament_api.ErrorHandling
{
    public class ApplicationErrors
    {
        public static Error ParseError()
        {
            return Error.Validation("Application.ParseError", "Could not parse the AccountId from token");
        }
    }
}
