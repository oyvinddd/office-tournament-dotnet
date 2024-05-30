namespace office_tournament_api.Validators
{
    public class TournamentResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }
        public string SucessMessage { get; set; }

        public TournamentResult()
        {
        }

        public TournamentResult(bool isValid, List<string> errors, string sucessMessage)
        {
            IsValid = isValid;
            Errors = errors;
            SucessMessage = sucessMessage;
        }
    }
}
