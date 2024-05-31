namespace office_tournament_api.Validators
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
        public string SucessMessage { get; set; }

        public ValidationResult()
        {
        }

        public ValidationResult(bool isValid, List<string> errors, string sucessMessage)
        {
            IsValid = isValid;
            Errors = errors;
            SucessMessage = sucessMessage;
        }
    }
}
