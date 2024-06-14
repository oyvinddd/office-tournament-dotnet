using System.ComponentModel.DataAnnotations.Schema;

namespace office_tournament_api.office_tournament_db
{
    public class Match
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(TournamentId))]
        public Tournament Tournament { get; set; }
        public Guid TournamentId { get; set; }

        [ForeignKey(nameof(WinnerId))]
        public TournamentAccount Winner {  get; set; }
        public Guid WinnerId { get; set; }
        public float WinnerDeltaScore { get; set; }

        [ForeignKey(nameof(LoserId))]
        public TournamentAccount Loser { get; set; }
        public Guid LoserId { get; set; }
        public float LoserDeltaScore { get; set; }
        public DateTime Date {  get; set; }
    }
}
