using office_tournament_api.Helpers;

namespace office_tournament_api.Services
{
    public class EloRating
    {
        public EloRating() { }

        /// <summary>
        /// Calculates the updated elo rating of two players after a match. Taken from: https://www.geeksforgeeks.org/elo-rating-algorithm/
        /// </summary>
        /// <param name="ratingPlayerA"></param>
        /// <param name="ratingPlayerB"></param>
        /// <param name="playerAWins"></param>
        public EloRatingResult CalculateEloRating(float ratingPlayerA, float ratingPlayerB, bool playerAWins)
        {
            EloRatingResult result = new EloRatingResult();
            float oldRatingPlayerA = ratingPlayerA;
            float oldRatingPlayerB = ratingPlayerB;

            int constant = 30;

            // To calculate the Winning
            // Probability of Player A
            float probPlayerA = Probability(ratingPlayerB, ratingPlayerA);

            // To calculate the Winning
            // Probability of Player B
            float probPlayerB = Probability(ratingPlayerA, ratingPlayerB);

            //When Player A wins
            if (playerAWins)
            {
                ratingPlayerA = ratingPlayerA + constant * (1 - probPlayerA);
                ratingPlayerB = ratingPlayerB + constant * (0 - probPlayerB);
            }            //When Player B wins
            else
            {
                ratingPlayerA = ratingPlayerA + constant * (0 - probPlayerA);
                ratingPlayerB = ratingPlayerB + constant * (1 - ratingPlayerB);
            }

            result.PlayerANewRating = ratingPlayerA;
            result.PlayerBNewRating = ratingPlayerB;
            result.WinnerPointsWon = ratingPlayerA - oldRatingPlayerA;
            result.LoserPointsLost = oldRatingPlayerB - ratingPlayerB;

            return result;
        }

        public float Probability(float rating1, float rating2)
        {
            float probability = 1.0f * 1.0f / (1 + 1.0f * (float)(Math.Pow(10, 1.0f * (rating1 - rating2) / 400)));
            return probability;
        }
    }
}
