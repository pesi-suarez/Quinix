using Quinix.Data;
using Quinix.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quinix
{
    public class FeatureMatrixGenerator
    {
        //TODONEW: Clase anidada a otro fichero.
        //TODONEW: Al generar la matriz de características ordenar alfabéticamente por equipo local dentro de cada jornada. De esta manera el resultado no depende de cómo se hayan almacenado los datos en la BD.
        //TODONEW: En el test unitario de WebScrapperToFile, en el que puede haber un problema similar, contar líneas en lugar de comparar hashes.
        //TODO: Tal vez debería generalizar lo de las performances de 3 5 10 a n0, n1...
        public class FeatureMatrixItem
        {
            private Season Season { get; set; }
            private Division Division { get; set; }
            private ClassificationTableItem GlobalHome { get; set; }
            private ClassificationTableItem Minileague3Home { get; set; }
            private ClassificationTableItem Minileague5Home { get; set; }
            private ClassificationTableItem Minileague10Home { get; set; }
            private ClassificationTableItem GlobalAway { get; set; }
            private ClassificationTableItem Minileague3Away { get; set; }
            private ClassificationTableItem Minileague5Away { get; set; }
            private ClassificationTableItem Minileague10Away { get; set; }
            private string Result { get; set; }

            public FeatureMatrixItem(Season season, Division division, ClassificationTableItem globalHome, ClassificationTableItem minileague3Home, ClassificationTableItem minileague5Home, ClassificationTableItem minileague10Home, ClassificationTableItem globalAway, ClassificationTableItem minileague3Away, ClassificationTableItem minileague5Away, ClassificationTableItem minileague10Away, string result)
            {
                Season = season;
                Division = division;
                GlobalHome = globalHome;
                Minileague3Home = minileague3Home;
                Minileague5Home = minileague5Home;
                Minileague10Home = minileague10Home;
                GlobalAway = globalAway;
                Minileague3Away = minileague3Away;
                Minileague5Away = minileague5Away;
                Minileague10Away = minileague10Away;
                Result = result;
            }

            public static string MakeHeader()
            {
                return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21}",
                    "season",
                    "division",
                    "home_team",
                    "away_team",
                    "match_day",
                    TeamAccumulator.MakeStreakHeader("total", "all_matches", "h"),
                    TeamAccumulator.MakeStreakHeader("total", "home", "h"),
                    TeamAccumulator.MakeNoStreakHeader("3", "all_matches", "h"),
                    TeamAccumulator.MakeNoStreakHeader("3", "home", "h"),
                    TeamAccumulator.MakeNoStreakHeader("5", "all_matches", "h"),
                    TeamAccumulator.MakeNoStreakHeader("5", "home", "h"),
                    TeamAccumulator.MakeNoStreakHeader("10", "all_matches", "h"),
                    TeamAccumulator.MakeNoStreakHeader("10", "home", "h"),
                    TeamAccumulator.MakeStreakHeader("total", "all_matches", "a"),
                    TeamAccumulator.MakeStreakHeader("total", "away", "a"),
                    TeamAccumulator.MakeNoStreakHeader("3", "all_matches", "a"),
                    TeamAccumulator.MakeNoStreakHeader("3", "away", "a"),
                    TeamAccumulator.MakeNoStreakHeader("5", "all_matches", "a"),
                    TeamAccumulator.MakeNoStreakHeader("5", "away", "a"),
                    TeamAccumulator.MakeNoStreakHeader("10", "all_matches", "a"),
                    TeamAccumulator.MakeNoStreakHeader("10", "away", "a"),
                    "result"
                );
            }

            public string ToFeatureMatrixString()
            {
                return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21}",
                    Season.StartYear,
                    Division.Name,
                    GlobalHome.GlobalAccumulator.Team.Name,
                    GlobalAway.GlobalAccumulator.Team.Name,
                    GlobalHome.MatchDay + 1,
                    GlobalHome.GlobalAccumulator.ToFeatureMatrixString(),
                    GlobalHome.HomeAccumulator.ToFeatureMatrixString(),
                    Minileague3Home.GlobalAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague3Home.HomeAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague5Home.GlobalAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague5Home.HomeAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague10Home.GlobalAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague10Home.HomeAccumulator.ToFeatureMatrixStringNoStreaks(),
                    GlobalAway.GlobalAccumulator.ToFeatureMatrixString(),
                    GlobalAway.AwayAccumulator.ToFeatureMatrixString(),
                    Minileague3Away.GlobalAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague3Away.AwayAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague5Away.GlobalAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague5Away.AwayAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague10Away.GlobalAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Minileague10Away.AwayAccumulator.ToFeatureMatrixStringNoStreaks(),
                    Result
                );
            }

        }

        private const string HOME_WIN = "1";
        private const string AWAY_WIN = "2";
        private const string TIE = "X";

        private Season CurrentSeason { get; set; }
        private Division CurrentDivision { get; set; }
        public List<FeatureMatrixItem> FeatureMatrix { get; set; }
        public string FeatureMatrixPath { get; set; }

        public FeatureMatrixGenerator(string featureMatrixPath)
        {
            FeatureMatrix = new List<FeatureMatrixItem>();
            FeatureMatrixPath = featureMatrixPath;
        }

        public void Execute()
        {
            IEnumerable<Season> seasons = GetSeasons();
            foreach (Season season in seasons)
            {
                CurrentSeason = season;
                ProcessSeason();
            }
        }

        private IEnumerable<Season> GetSeasons()
        {
            QuinixDbContext context = new QuinixDbContext();
            return context.Seasons.ToList();
        }

        private void ProcessSeason()
        {
            IEnumerable<Division> divisions = GetDivisions();
            foreach (Division division in divisions)
            {
                CurrentDivision = division;
                ProcessDivision();
            }
        }

        private IEnumerable<Division> GetDivisions()
        {
            QuinixDbContext context = new QuinixDbContext();
            return context.Divisions.ToList();
        }

        private void ProcessDivision()
        {
            int lastMatchDay = GetLastMatchDay();
            ClassificationTable globalClassification = new ClassificationTable(CurrentSeason, CurrentDivision, 1, lastMatchDay, lastMatchDay);
            globalClassification.GenerateClassificationTable();
            int startingMatchDay = 11;
            //TODO: Parametrizar la jornada inicial a partir del número de "miniligas". P. ej. si tengo una miniliga de 10 hacia atrás, debo empezar enl a jornada 11.
            foreach (int matchDay in Enumerable.Range(startingMatchDay, lastMatchDay - startingMatchDay + 1))
            {
                int start3 = Math.Max(matchDay - 3, 1);
                ClassificationTable minileague3 = new ClassificationTable(CurrentSeason, CurrentDivision, start3, 3, lastMatchDay);
                minileague3.GenerateClassificationTable();

                int start5 = Math.Max(matchDay - 5, 1);
                ClassificationTable minileague5 = new ClassificationTable(CurrentSeason, CurrentDivision, start5, 5, lastMatchDay);
                minileague5.GenerateClassificationTable();

                int start10 = Math.Max(matchDay - 10, 1);
                ClassificationTable minileague10 = new ClassificationTable(CurrentSeason, CurrentDivision, start10, 10, lastMatchDay);
                minileague10.GenerateClassificationTable();

                IEnumerable<Match> matches = Utils.GetMatches(CurrentSeason, CurrentDivision, matchDay);
                foreach (Match match in matches)
                {
                    ProcessMatch(match, globalClassification, minileague3, minileague5, minileague10);
                }
            }
        }

        //Obtiene la última jornada de la temporada (distingue entre divisiones y entre si la temporada ha terminado o no).
        private int GetLastMatchDay()
        {
            QuinixDbContext context = new QuinixDbContext();
            return context.Matches.Where(m => m.SeasonId == CurrentSeason.Id && m.DivisionId == CurrentDivision.Id).Select(m => m.MatchDay).Max();
        }

        private void ProcessMatch(Match match, ClassificationTable globalClassification, ClassificationTable minileague3, ClassificationTable minileague5, ClassificationTable minileague10)
        {
            Console.WriteLine(match.MatchDay);
            string homeTeamName = match.HomeTeam.Name;
            string awayteamName = match.AwayTeam.Name;
            string result = (match.HomeTeamGoals > match.AwayTeamGoals) ? HOME_WIN : (match.HomeTeamGoals < match.AwayTeamGoals) ? AWAY_WIN : TIE;
            var gh = globalClassification.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(homeTeamName)).Single();
            var l3h = minileague3.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(homeTeamName)).Single();
            var l5h = minileague5.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(homeTeamName)).Single();
            var l10h = minileague10.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(homeTeamName)).Single();
            var ga = globalClassification.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(awayteamName)).Single();
            var l3a = minileague3.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(awayteamName)).Single();
            var l5a = minileague5.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(awayteamName)).Single();
            var l10a = minileague10.History.Where(h => h.MatchDay == match.MatchDay - 1 && h.GlobalAccumulator.Team.Name.Equals(awayteamName)).Single();
            FeatureMatrixItem x = new FeatureMatrixItem(CurrentSeason, CurrentDivision, gh, l3h, l5h, l10h, ga, l3a, l5a, l10a, result);

            FeatureMatrix.Add(x);
        }

        public void PrintFeatureMatrix()
        {
            using (StreamWriter writer = File.CreateText(FeatureMatrixPath))
            {
                writer.WriteLine(FeatureMatrixItem.MakeHeader());
                foreach (FeatureMatrixItem item in FeatureMatrix)
                    writer.WriteLine(item.ToFeatureMatrixString());
            }

        }

        public void ProcessSingle(int seasonYear, string divisionName)
        {
            CurrentSeason = GetSeasons().Where(s => s.StartYear == seasonYear).Single();
            CurrentDivision = GetDivisions().Where(d => d.Name.Equals(divisionName)).Single();
            ProcessDivision();
        }

    }
}
