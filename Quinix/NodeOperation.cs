using HtmlAgilityPack;
using Quinix.Data;
using Quinix.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Quinix
{
    public class NodeOperation
    {
        public static void WriteToFile(int year, string division, int matchDay, HtmlNode resultNode, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, append: true))
            {
                List<string> data = GetTeamsAndResult(resultNode);
                string homeTeam = data[0];
                string awayTeam = data[1];
                string result = data[2];

                //Partido todavía sin jugar (posiblemente aplazado). No hacemos nada.
                Regex validResult = new Regex(@"\d+-\d+");
                if (!validResult.Match(result).Success)
                    return;

                string matchDayInfo = string.Format("Temporada {0} {1} división. Jornada {2}", Utils.YearToSeasonString(year), division, matchDay);

                string separator1 = homeTeam.Length < 12 ? (homeTeam.Length < 8 ? "\t\t\t" : "\t\t") : "\t";
                string separator2 = "\t\t";
                sw.WriteLine("{0}:\t\t{1}{2}{3}{4}{5}", matchDayInfo, homeTeam, separator1, result, separator2, awayTeam);
            }
        }

        private static List<string> GetTeamsAndResult(HtmlNode resultNode)
        {
            List<string> data = new List<string> { string.Empty, string.Empty, string.Empty };
            foreach (HtmlNode child in resultNode.ChildNodes)
            {
                if (child.HasAttributes) //Es como si los saltos de línea se interpretaran como nodos hijos.
                {
                    switch (child.Attributes["class"].Value)
                    {
                        case "equipo-local":
                            data[0] = child.InnerText;
                            break;
                        case "equipo-visitante":
                            data[1] = child.InnerText;
                            break;
                        case "resultado":
                            data[2] = child.InnerText;
                            break;
                    }
                }
            }
            return data;
        }

        public static void SaveToDb(int year, string divisionName, int matchDay, HtmlNode resultNode, string filePath)
        {
            List<string> data = GetTeamsAndResult(resultNode);
            string homeTeamName = data[0];
            string awayTeamName = data[1];
            string resultString = data[2];

            //Partido todavía sin jugar (posiblemente aplazado). No hacemos nada.
            Regex validResult = new Regex(@"\d+-\d+");
            if (!validResult.Match(resultString).Success)
                return;

            int homeTeamGoals = int.Parse(resultString.Split('-')[0]);
            int awayTeamGoals = int.Parse(resultString.Split('-')[1]);

            QuinixDbContext context = new QuinixDbContext();
            Division division = context.Divisions.Single(d => d.Name.Equals(divisionName));

            Season season;
            if (context.Seasons.Count(s => s.StartYear == year) == 0)
            {
                season = new Season
                {
                    StartYear = year,
                    EndYear = year + 1
                };
                context.Seasons.Add(season);
            }
            else
                season = context.Seasons.Single(s => s.StartYear == year);

            Team homeTeam;
            if (context.Teams.Count(t => t.Name.Equals(homeTeamName)) == 0)
            {
                homeTeam = new Team
                {
                    Name = homeTeamName
                };
                context.Teams.Add(homeTeam);
            }
            else
                homeTeam = context.Teams.Single(t => t.Name.Equals(homeTeamName));

            Team awayTeam;
            if (context.Teams.Count(t => t.Name.Equals(awayTeamName)) == 0)
            {
                awayTeam = new Team
                {
                    Name = awayTeamName
                };
                context.Teams.Add(awayTeam);
            }
            else
                awayTeam = context.Teams.Single(t => t.Name.Equals(awayTeamName));

            if (!MatchExists(context, season, division, homeTeam, awayTeam))
            {
                Model.Match match = new Model.Match
                {
                    Season = season,
                    Division = division,
                    MatchDay = matchDay,
                    HomeTeam = homeTeam,
                    AwayTeam= awayTeam,
                    HomeTeamGoals = homeTeamGoals,
                    AwayTeamGoals = awayTeamGoals
                };

                context.Matches.Add(match);
            }

            context.SaveChanges();
        }

        private static bool MatchExists(QuinixDbContext context, Season season, Division division, Team homeTeam, Team awayTeam)
        {
            return context.Matches.Any(
                m => m.SeasonId == season.Id &&
                m.DivisionId == division.Id &&
                m.HomeTeamId == homeTeam.Id &&
                m.AwayTeamId == awayTeam.Id);
        }
    }
}
