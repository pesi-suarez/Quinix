namespace Quinix.Model
{
    public class Match
    {
        public int Id { get; set; }
        public Season Season { get; set; }
        public int SeasonId { get; set; }
        public Division Division { get; set; }
        public int DivisionId { get; set; }
        public int MatchDay { get; set; }
        public Team HomeTeam { get; set; }
        public int HomeTeamId { get; set; }
        public Team AwayTeam { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }
    }
}
