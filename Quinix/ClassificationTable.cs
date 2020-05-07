using Quinix.Data;
using Quinix.Model;
using System.Collections.Generic;
using System.Linq;

namespace Quinix
{
    public class ClassificationTable
    {
        public List<ClassificationTableItem> History { get; set; }
        private Dictionary<string, TeamAccumulator> GlobalAccumulators { get; set; }
        private Dictionary<string, TeamAccumulator> HomeAccumulators { get; set; }
        private Dictionary<string, TeamAccumulator> AwayAccumulators { get; set; }
        public Season Season { get; set; }
        public Division Division { get; set; }
        public int InitialMatchDay { get; set; }
        public int TotalMatchDays { get; set; }
        public int LastMatchDay { get; set; }

        public ClassificationTable(Season season, Division division, int initialMatchDay, int totalMatchDays, int lastMatchDay)
        {
            InitialMatchDay = initialMatchDay;
            TotalMatchDays = totalMatchDays;
            LastMatchDay = lastMatchDay;
            QuinixDbContext context = new QuinixDbContext();
            Season = season;
            Division = division;
            History = new List<ClassificationTableItem>();
            GlobalAccumulators = new Dictionary<string, TeamAccumulator>();
            HomeAccumulators = new Dictionary<string, TeamAccumulator>();
            AwayAccumulators = new Dictionary<string, TeamAccumulator>();
            List<Team> homeTeamsMatchDay1 = Utils.GetMatches(Season, Division, 1).Select(m => m.HomeTeam).ToList();
            List<Team> awayTeamsMatchDay1 = Utils.GetMatches(Season, Division, 1).Select(m => m.AwayTeam).ToList();
            List<Team> teams = homeTeamsMatchDay1.Concat(awayTeamsMatchDay1).ToList();
            foreach (Team team in teams)
            {
                GlobalAccumulators.Add(team.Name, new TeamAccumulator(team));
                HomeAccumulators.Add(team.Name, new TeamAccumulator(team));
                AwayAccumulators.Add(team.Name, new TeamAccumulator(team));
            }
        }

        public void GenerateClassificationTable()
        {
            QuinixDbContext context = new QuinixDbContext();
            foreach (int matchDay in Enumerable.Range(InitialMatchDay, TotalMatchDays))
            {
                List<Match> matches = Utils.GetMatches(Season, Division, matchDay);
                foreach (Match g in matches)
                {
                    Team homeTeam = g.HomeTeam;
                    Team awayTeam = g.AwayTeam;
                    GlobalAccumulators[homeTeam.Name].GamesPlayed++;
                    HomeAccumulators[homeTeam.Name].GamesPlayed++;
                    GlobalAccumulators[awayTeam.Name].GamesPlayed++;
                    AwayAccumulators[awayTeam.Name].GamesPlayed++;
                    if (g.HomeTeamGoals > g.AwayTeamGoals)
                    {
                        TeamWon(GlobalAccumulators[homeTeam.Name], g.AwayTeamGoals);
                        TeamWon(HomeAccumulators[homeTeam.Name], g.AwayTeamGoals);
                        TeamLost(GlobalAccumulators[awayTeam.Name], g.AwayTeamGoals);
                        TeamLost(AwayAccumulators[awayTeam.Name], g.AwayTeamGoals);
                    }
                    else if (g.HomeTeamGoals < g.AwayTeamGoals)
                    {
                        TeamWon(GlobalAccumulators[awayTeam.Name], g.HomeTeamGoals);
                        TeamWon(AwayAccumulators[awayTeam.Name], g.HomeTeamGoals);
                        TeamLost(GlobalAccumulators[homeTeam.Name], g.HomeTeamGoals);
                        TeamLost(HomeAccumulators[homeTeam.Name], g.HomeTeamGoals);
                    }
                    else
                    {
                        TeamTied(GlobalAccumulators[homeTeam.Name], g.HomeTeamGoals);
                        TeamTied(HomeAccumulators[homeTeam.Name], g.HomeTeamGoals);
                        TeamTied(GlobalAccumulators[awayTeam.Name], g.AwayTeamGoals);
                        TeamTied(AwayAccumulators[awayTeam.Name], g.AwayTeamGoals);
                    }
                    GlobalAccumulators[homeTeam.Name].GoalsInFavour += g.HomeTeamGoals;
                    HomeAccumulators[homeTeam.Name].GoalsInFavour += g.HomeTeamGoals;
                    GlobalAccumulators[homeTeam.Name].GoalsAgainst += g.AwayTeamGoals;
                    HomeAccumulators[homeTeam.Name].GoalsAgainst += g.AwayTeamGoals;
                    GlobalAccumulators[awayTeam.Name].GoalsInFavour += g.AwayTeamGoals;
                    AwayAccumulators[awayTeam.Name].GoalsInFavour += g.AwayTeamGoals;
                    GlobalAccumulators[awayTeam.Name].GoalsAgainst += g.HomeTeamGoals;
                    AwayAccumulators[awayTeam.Name].GoalsAgainst += g.HomeTeamGoals;
                }

                //TODO: Las reglas para desempatar o para asignar puntos podrían depender de la temporada, si me voy muy atrás.
                List<TeamAccumulator> classif = GlobalAccumulators.Values.OrderByDescending(t => t.Points).ToList();
                List<TeamAccumulator> aux = TieBreak(classif);
                while (!EqualClassifications(classif, aux))
                {
                    classif = aux;
                    aux = TieBreak(classif);
                }
                classif = aux; //Por si no hay ningún desempate que hacer.

                int i = 1;
                foreach (TeamAccumulator accumulator in classif)
                {
                    History.Add(new ClassificationTableItem(i, matchDay, new TeamAccumulator(accumulator), new TeamAccumulator(HomeAccumulators[accumulator.Team.Name]), new TeamAccumulator(AwayAccumulators[accumulator.Team.Name])));
                    i++;
                }
            }
        }

        private void TeamWon(TeamAccumulator accumulator, int goalsConceded)
        {
            accumulator.GamesWon++;
            accumulator.WinningStreak++;
            accumulator.TieingStreak = 0;
            accumulator.LosingStreak = 0;
            accumulator.UndefeatedStreak++;
            accumulator.NonWinningStreak = 0;
            TeamScored(accumulator);
            accumulator.Points += 3;
            if (goalsConceded > 0)
                TeamConceded(accumulator);
            else
                CleanSheet(accumulator);
        }

        private void TeamScored(TeamAccumulator accumulator)
        {
            accumulator.ScoringStreak++;
            accumulator.NonScoringStreak = 0;
        }

        private void TeamDidNotScore(TeamAccumulator accumulator)
        {
            accumulator.ScoringStreak = 0;
            accumulator.NonScoringStreak++;
        }

        private void TeamConceded(TeamAccumulator accumulator)
        {
            accumulator.CleanSheetStreak = 0;
            accumulator.ConcedingStreak++;
        }

        private void CleanSheet(TeamAccumulator accumulator)
        {
            accumulator.CleanSheetStreak++;
            accumulator.ConcedingStreak = 0;
        }

        private void TeamLost(TeamAccumulator accumulator, int goalsScored)
        {
            accumulator.GamesLost++;
            accumulator.WinningStreak = 0;
            accumulator.TieingStreak = 0;
            accumulator.LosingStreak++;
            accumulator.UndefeatedStreak = 0;
            accumulator.NonWinningStreak++;
            if (goalsScored > 0)
                TeamScored(accumulator);
            else
                TeamDidNotScore(accumulator);
            TeamConceded(accumulator);
        }

        private void TeamTied(TeamAccumulator accumulator, int goalsScored)
        {
            accumulator.GamesTied++;
            accumulator.WinningStreak = 0;
            accumulator.TieingStreak++;
            accumulator.LosingStreak = 0;
            accumulator.Points++;
            accumulator.UndefeatedStreak++;
            accumulator.NonWinningStreak++;
            if (goalsScored > 0)
            {
                TeamScored(accumulator);
                TeamConceded(accumulator);
            }
            else
            {
                TeamDidNotScore(accumulator);
                CleanSheet(accumulator);
            }
        }

        //Recorre la tabla de clasificaciones parcialmente ordenada "classif" de manera secuencial, resolviendo los empates que va encontrando. Pueden ser necesarias varias invocaciones hasta que la tabla esté finalmente ordenada.
        private List<TeamAccumulator> TieBreak(List<TeamAccumulator> classif)
        {
            List<TeamAccumulator> result = new List<TeamAccumulator>();
            TeamAccumulator previous = classif.First();
            TeamAccumulator current = new TeamAccumulator();
            foreach (TeamAccumulator item in classif.Skip(1))
            {
                current = item;
                if (OrderOk(previous, current))
                {
                    result.Add(previous);
                    previous = current;
                }
                else
                    result.Add(current);
            }
            if (result.Last() == current)
                result.Add(previous);
            else
                result.Add(current);
            return result;
        }

        //Devuelve "true" si es correcto que el equipo del acumulador "previous" esté por delante en la tabla que el equipo del acumulador "current".
        private bool OrderOk(TeamAccumulator previous, TeamAccumulator current)
        {
            //Puntos
            if (previous.Points > current.Points)
                return true;

            //Enfrentamientos directos
            bool? particularRecordTieBreak = GenerateParticularRecordTieBreak(previous, current);
            if (particularRecordTieBreak.HasValue)
                return particularRecordTieBreak.Value;

            //Goal Average
            if (previous.GoalsInFavour - previous.GoalsAgainst > current.GoalsInFavour - current.GoalsAgainst)
                return true;
            else if (previous.GoalsInFavour - previous.GoalsAgainst < current.GoalsInFavour - current.GoalsAgainst)
                return false;

            //Goles a favor
            if (previous.GoalsInFavour > current.GoalsInFavour)
                return true;
            else if (previous.GoalsInFavour < current.GoalsInFavour)
                return false;

            //Empate total
            return true;
        }

        //Si dos equipos tienen los mismos puntos y han jugado ya dos veces entre ellos, la diferencia de goles en esos dos partidos sirve para desempatar. No se aplica la regla de que valen más los goles anotados como visitante en caso de empate. Ej (1-1), (3-3): el que marcó 3 como visitante no queda por delante. Si no se puede desempatar se devuelve null.
        private bool? GenerateParticularRecordTieBreak(TeamAccumulator previous, TeamAccumulator current)
        {
            QuinixDbContext context = new QuinixDbContext();
            List<Match> ParticularRecord = Utils.GetMatches(Season, Division, 1, LastMatchDay).Where(m => m.HomeTeamId == previous.Team.Id && m.AwayTeamId == current.Team.Id || m.AwayTeamId == previous.Team.Id && m.HomeTeamId == current.Team.Id).ToList();
            if (ParticularRecord.Count() == 2)
            {
                Match firstMatch = ParticularRecord.First();
                Match secondMatch = ParticularRecord.Last();
                bool previousAsHomeFirstMatch = firstMatch.HomeTeamId == previous.Team.Id;
                if (previousAsHomeFirstMatch)
                {
                    if (firstMatch.HomeTeamGoals + secondMatch.AwayTeamGoals > firstMatch.AwayTeamGoals + secondMatch.HomeTeamGoals)
                        return true;
                    else if (firstMatch.HomeTeamGoals + secondMatch.AwayTeamGoals < firstMatch.AwayTeamGoals + secondMatch.HomeTeamGoals)
                        return false;
                }
                else
                {
                    if (firstMatch.HomeTeamGoals + secondMatch.AwayTeamGoals > firstMatch.AwayTeamGoals + secondMatch.HomeTeamGoals)
                        return false;
                    else if (firstMatch.HomeTeamGoals + secondMatch.AwayTeamGoals < firstMatch.AwayTeamGoals + secondMatch.HomeTeamGoals)
                        return true;
                }
            }
            return null;
        }

        private bool EqualClassifications(List<TeamAccumulator> classif, List<TeamAccumulator> aux)
        {
            TeamAccumulator[] classifArray = classif.ToArray();
            TeamAccumulator[] auxArray = aux.ToArray();
            for (int i = 0; i < classif.Count(); i++)
                if (!classifArray[i].Team.Name.Equals(auxArray[i].Team.Name))
                    return false;
            return true;
        }

    }
}
