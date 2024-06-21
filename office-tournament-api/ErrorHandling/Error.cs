namespace office_tournament_api.ErrorHandling
{
    public class Error
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public ErrorType ErrorType { get; set; }

        public Error() { }
        public Error(string code, string description, ErrorType errorType)
        {
            Code = code;
            Description = description;
            ErrorType = errorType;
        }

        public static Error None()
        {
            Error newError = new Error(string.Empty, string.Empty, ErrorType.None);
            return newError;
        }

        public static Error NotFound(string code, string description)
        {
            Error newError = new Error(code, description, ErrorType.NotFound);
            return newError;
        }

        public static Error Validation(string code, string description)
        {
            Error newError = new Error(code, description, ErrorType.Validation);
            return newError;
        }
        public static Error Conflict(string code, string description)
        {
            Error newError = new Error(code, description, ErrorType.Conflict);
            return newError;
        }
        public static Error Failure(string code, string description)
        {
            Error newError = new Error(code, description, ErrorType.Failure);
            return newError;
        }
    }

    public enum ErrorType
    {
        None = 0,
        Failure = 1,
        Validation = 2,
        NotFound = 3,
        Conflict = 4
    }
}
