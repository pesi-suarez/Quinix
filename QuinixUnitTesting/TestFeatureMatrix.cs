using Deedle;
using Xunit;

//TODO: De momento sólo puedo tener test unitarios con equipos que no tienen partidos cambiados de fecha. Habría que cambiar el algoritmo de manera que los acumuladores se hacen por fecha y no por jornada para tener en cuenta esta peculiaridad.
namespace QuinixUnitTesting
{
    public class TestFeatureMatrix
    {
        private readonly string _resultsFilePath = @"..\..\..\test_data\TestFeatureMatrix\features.csv";

        //TODO: Antes de la ejecución de los tests se deberían descargar los datos y regenerar la matriz de características.
        private float GetTarget(int season, string division, string homeTeam, int matchDay, string column)
        {
            var frame = Frame.ReadCsv(_resultsFilePath, separators: ";");
            var targetStr = frame.Rows.Where(r =>
                int.Parse(r.Value["season"].ToString()) == season &&
                r.Value["division"].Equals(division) &&
                r.Value["home_team"].Equals(homeTeam) &&
                int.Parse(r.Value["match_day"].ToString()) == matchDay
            ).FirstValue()[column];
            return float.Parse(targetStr.ToString());
        }

        //TODONEW: Mencionar que el DF para testear los datos debería contener las observaciones de la temporada 2016
        [Fact]
        public void Sevilla2016Primera25_total_all_matches_h_points_ratio()
        {
            float target = GetTarget(2016, "primera", "Sevilla", 25, "total_all_matches_h_points_ratio");
            Assert.Equal(0.722, target, 3);
        }

        [Fact]
        public void Valencia2016Primera25_total_all_matches_h_win_ratio()
        {
            float target = GetTarget(2016, "primera", "Valencia", 25, "total_all_matches_h_win_ratio");
            Assert.Equal(0.292, target, 3);
        }

        [Fact]
        public void Osasuna2016Primera25_total_all_matches_h_tie_ratio()
        {
            float target = GetTarget(2016, "primera", "Osasuna", 25, "total_all_matches_h_tie_ratio");
            Assert.Equal(0.292, target, 3);
        }

        [Fact]
        public void Osasuna2016Primera25_total_all_matches_h_lost_ratio()
        {
            float target = GetTarget(2016, "primera", "Osasuna", 25, "total_all_matches_h_lost_ratio");
            Assert.Equal(0.667, target, 3);
        }

        [Fact]
        public void Osasuna2016Primera25_total_all_matches_h_goals_in_favour_ratio()
        {
            float target = GetTarget(2016, "primera", "Osasuna", 25, "total_all_matches_h_goals_in_favour_ratio");
            Assert.Equal(1, target, 3);
        }

        [Fact]
        public void Sevilla2016Primera25_total_all_matches_h_goals_against_ratio()
        {
            float target = GetTarget(2016, "primera", "Sevilla", 25, "total_all_matches_h_goals_against_ratio");
            Assert.Equal(1.208, target, 3);
        }

        [Fact]
        public void Osasuna2016Primera25_total_all_matches_h_winning_streak()
        {
            float target = GetTarget(2016, "primera", "Osasuna", 25, "total_all_matches_h_winning_streak");
            Assert.Equal(0, target, 3);
        }

        [Fact]
        public void RSociedad2016Primera25_total_all_matches_h_tieing_streak()
        {
            float target = GetTarget(2016, "primera", "R. Sociedad", 25, "total_all_matches_h_tieing_streak");
            Assert.Equal(0, target, 3);
        }

        [Fact]
        public void Osasuna2016Primera25_total_all_matches_h_losing_streak()
        {
            float target = GetTarget(2016, "primera", "Osasuna", 25, "total_all_matches_h_losing_streak");
            Assert.Equal(4, target, 3);
        }

        [Fact]
        public void Malaga2016Primera25_total_all_matches_h_undefeated_streak()
        {
            float target = GetTarget(2016, "primera", "Málaga", 25, "total_all_matches_h_undefeated_streak");
            Assert.Equal(0, target, 3);
        }

        [Fact]
        public void RSociedad2016Primera25_total_all_matches_h_non_winning_streak()
        {
            float target = GetTarget(2016, "primera", "R. Sociedad", 25, "total_all_matches_h_non_winning_streak");
            Assert.Equal(0, target, 3);
        }

        [Fact]
        public void Sevilla2016Primera25_total_all_matches_h_scoring_streak()
        {
            float target = GetTarget(2016, "primera", "Sevilla", 25, "total_all_matches_h_scoring_streak");
            Assert.Equal(3, target, 3);
        }

        [Fact]
        public void Granada2016Primera25_total_all_matches_h_non_scoring_streak()
        {
            float target = GetTarget(2016, "primera", "Granada", 25, "total_all_matches_h_non_scoring_streak");
            Assert.Equal(0, target, 3);
        }

        [Fact]
        public void Granada2016Primera25_total_all_matches_h_clean_sheet_streak()
        {
            float target = GetTarget(2016, "primera", "Granada", 25, "total_all_matches_h_clean_sheet_streak");
            Assert.Equal(0, target, 3);
        }

        [Fact]
        public void Granada2016Primera25_total_all_matches_h_conceding_streak()
        {
            float target = GetTarget(2016, "primera", "Granada", 25, "total_all_matches_h_conceding_streak");
            Assert.Equal(3, target, 3);
        }

    }
}
