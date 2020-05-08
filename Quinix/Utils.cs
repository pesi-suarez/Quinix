using Microsoft.EntityFrameworkCore;
using Quinix.Data;
using Quinix.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Quinix
{
    public class Utils
    {
        public static string YearToSeasonString(int startingYear)
        {
            string finishingYearSuffix = ((startingYear + 1) % 100).ToString("D2");
            return string.Format("{0}_{1}", startingYear, finishingYearSuffix);
        }

        public static List<Match> GetMatches(Season season, Division division, int matchDay)
        {
            return GetMatches(season, division, matchDay, matchDay);
        }

        public static List<Match> GetMatches(Season season, Division division, int firstMatchDay, int lastMatchDay)
        {
            QuinixDbContext context = new QuinixDbContext();
            return context.Matches.Include(m => m.HomeTeam).Include(m => m.AwayTeam).Where(m => m.SeasonId == season.Id && m.DivisionId == division.Id && m.MatchDay >= firstMatchDay && m.MatchDay <= lastMatchDay).OrderBy(m => m.MatchDay).ToList();
        }

        //Genera un hash MD5 de 32 caracteres a partir del contenido del fichero situado en "filePath".
        public static string GenerateFileHash(string filePath)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(filePath))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }

    }
}
