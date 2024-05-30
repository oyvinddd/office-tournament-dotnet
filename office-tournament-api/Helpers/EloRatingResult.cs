namespace office_tournament_api.Helpers
{
    public class EloRatingResult
    {
        public float PlayerANewRating { get; set; }
        public float PlayerBNewRating { get; set; }
        public float LoserPointsLost { get; set; }
        public float WinnerPointsWon { get; set; }
        public EloRatingResult() { }

        public EloRatingResult(float playerANewRating, float playerBNewRating, float loserPointsLost, float winnerPointsWon)
        {
            PlayerANewRating = playerANewRating;
            PlayerBNewRating = playerBNewRating;
            LoserPointsLost = loserPointsLost;
            WinnerPointsWon = winnerPointsWon;
        }
    }
}
