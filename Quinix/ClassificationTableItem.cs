namespace Quinix
{
    public class ClassificationTableItem
    {
        public int MatchDay { get; set; }
        public int Position { get; set; }
        public TeamAccumulator GlobalAccumulator { get; set; }
        public TeamAccumulator HomeAccumulator { get; set; }
        public TeamAccumulator AwayAccumulator { get; set; }

        public ClassificationTableItem(int position, int matchDay, TeamAccumulator globalAccumulator, TeamAccumulator homeAccumulator, TeamAccumulator awayAccumulator)
        {
            MatchDay = matchDay;
            Position = position;
            GlobalAccumulator = globalAccumulator;
            HomeAccumulator = homeAccumulator;
            AwayAccumulator = awayAccumulator;
        }

        public string ToCsvString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}", MatchDay, Position, GlobalAccumulator.Team.Name, GlobalAccumulator.ToCsvString(), HomeAccumulator.ToCsvString(), AwayAccumulator.ToCsvString());
        }

    }
}
