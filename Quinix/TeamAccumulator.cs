using Quinix.Model;

namespace Quinix
{
    public class TeamAccumulator
    {
        public Team Team { get; set; }
        public int GamesPlayed { get; set; }
        public int Points { get; set; }
        public int GamesWon { get; set; }
        public int GamesTied { get; set; }
        public int GamesLost { get; set; }
        public int GoalsInFavour { get; set; }
        public int GoalsAgainst { get; set; }
        public int WinningStreak { get; set; }
        public int TieingStreak { get; set; }
        public int LosingStreak { get; set; }
        public int UndefeatedStreak { get; set; }
        public int NonWinningStreak { get; set; }
        public int ScoringStreak { get; set; }
        public int NonScoringStreak { get; set; }
        public int CleanSheetStreak { get; set; }
        public int ConcedingStreak { get; set; }

        public TeamAccumulator() { }

        public TeamAccumulator(Team team)
        {
            Team = team;
        }

        public TeamAccumulator(TeamAccumulator accumulator)
        {
            Team = accumulator.Team;
            GamesPlayed = accumulator.GamesPlayed;
            Points = accumulator.Points;
            GamesWon = accumulator.GamesWon;
            GamesTied = accumulator.GamesTied;
            GamesLost = accumulator.GamesLost;
            GoalsInFavour = accumulator.GoalsInFavour;
            GoalsAgainst = accumulator.GoalsAgainst;
            WinningStreak = accumulator.WinningStreak;
            TieingStreak = accumulator.TieingStreak;
            LosingStreak = accumulator.LosingStreak;
            UndefeatedStreak = accumulator.UndefeatedStreak;
            NonWinningStreak = accumulator.NonWinningStreak;
            ScoringStreak = accumulator.ScoringStreak;
            NonScoringStreak = accumulator.NonScoringStreak;
            CleanSheetStreak = accumulator.CleanSheetStreak;
            ConcedingStreak = accumulator.ConcedingStreak;
        }

        public string ToCsvString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15}", GamesPlayed, Points, GamesWon, GamesTied, GamesLost, GoalsInFavour, GoalsAgainst, WinningStreak, TieingStreak, LosingStreak, UndefeatedStreak, NonWinningStreak, ScoringStreak, NonScoringStreak, CleanSheetStreak, ConcedingStreak);
        }

        public static string MakeStreakHeader(string minileaguePrefix, string accumulatorPrefix, string homeOrAwayPrefix)
        {
            string prefix = minileaguePrefix + "_" + accumulatorPrefix + "_" + homeOrAwayPrefix;
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                MakeNoStreakHeader(minileaguePrefix, accumulatorPrefix, homeOrAwayPrefix),
                prefix + "_" + "winning_streak",
                prefix + "_" + "tieing_streak",
                prefix + "_" + "losing_streak",
                prefix + "_" + "undefeated_streak",
                prefix + "_" + "non_winning_streak",
                prefix + "_" + "scoring_streak",
                prefix + "_" + "non_scoring_streak",
                prefix + "_" + "clean_sheet_streak",
                prefix + "_" + "conceding_streak"
                );
        }

        public static string MakeNoStreakHeader(string minileaguePrefix, string accumulatorPrefix, string homeOrAwayPrefix)
        {
            string prefix = minileaguePrefix + "_" + accumulatorPrefix + "_" + homeOrAwayPrefix;
            return string.Format("{0};{1};{2};{3};{4};{5}",
                prefix + "_" + "points_ratio",
                prefix + "_" + "win_ratio",
                prefix + "_" + "tie_ratio",
                prefix + "_" + "lost_ratio",
                prefix + "_" + "goals_in_favour_ratio",
                prefix + "_" + "goals_against_ratio"
                );
        }

        public string ToFeatureMatrixString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", ToFeatureMatrixStringNoStreaks(), WinningStreak, TieingStreak, LosingStreak, UndefeatedStreak, NonWinningStreak, ScoringStreak, NonScoringStreak, CleanSheetStreak, ConcedingStreak);
        }

        public string ToFeatureMatrixStringNoStreaks()
        {
            //Puede petar si el equipo todavía no ha jugado como local o como visitante.
            //Podría arreglarlo aquí, pero creo que es mejor empezar a contar desde una jornada en que todos ya hayan jugado como local y como visitante.
            float pointsRatio = ((float)Points) / (3 * GamesPlayed);
            float winRatio = ((float)GamesWon) / GamesPlayed;
            float tieRatio = ((float)GamesTied) / GamesPlayed;
            float lostRatio = ((float)GamesLost) / GamesPlayed;
            float goalsInFavourRatio = ((float)GoalsInFavour) / GamesPlayed;
            float goalsAgainstRatio = ((float)GoalsAgainst) / GamesPlayed;
            return string.Format("{0};{1};{2};{3};{4};{5}", pointsRatio, winRatio, tieRatio, lostRatio, goalsInFavourRatio, goalsAgainstRatio);
        }

    }
}
