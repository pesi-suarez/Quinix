﻿using HtmlAgilityPack;  //TODONEW: Mencionar en el readme.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Quinix
{
    //TODONEW: Repasar los TODO antiguos -> Todos los que quedan merecen estar, al menos en la versión de Bitbucket.
    public class FootballMatchesWebScrapper
    {
        //TODO: Explorar las temporadas anteriores a 2001_02
        public const int FIRST_SEASON = 2001;
        private readonly string[] DIVISIONS = { "primera", "segunda" }; //NO MODIFICAR (URL)
        //TODONEW: Anonimizar la URL. Igualmente me he informado de que el scrapping web es legal en españa: https://diariodeuneletrado.wordpress.com/2017/03/22/es-legal-el-web-scrapping-de-webscrapping-y-legalidad/
        private const string ROOT_URL = "https://www.marca.com/estadisticas/futbol";
        private const string ENCODING = "iso-8859-1";

        public int InitialSeason { get; set; }
        public int FinalSeason { get; set; }
        public int CurrentYear { get; set; }
        public string CurrentDivision { get; set; }
        public int CurrentMatchDay { get; set; }
        public Action<int, string, int, HtmlNode, string> NodeOperation { get; set; }
        public string ResultsFilePath { get; set; }

        public FootballMatchesWebScrapper(int initialSeason, int finalSeason, Action<int, string, int, HtmlNode, string> nodeOperation, string resultsFilePath)
        {
            InitialSeason = initialSeason;
            FinalSeason = finalSeason;
            NodeOperation = nodeOperation;
            ResultsFilePath = resultsFilePath;
        }

        public void ExecuteAll()
        {
            foreach (int year in Enumerable.Range(InitialSeason, FinalSeason - InitialSeason + 1))
            {
                CurrentYear = year;
                ProcessCurrentYear();
            }
        }

        private void ProcessCurrentYear()
        {
            foreach (string division in DIVISIONS)
            {
                CurrentDivision = division;
                ProcessCurrentDivision();
            }
        }

        private void ProcessCurrentDivision()
        {
            int finalMatchDay = CurrentDivision.Equals("primera") ? 38 : 42;
            foreach (int matchDay in Enumerable.Range(1, finalMatchDay))
            {
                CurrentMatchDay = matchDay;
                ProcessCurrentMatchDay();
            }
        }

        private void ProcessCurrentMatchDay()
        {
            string matchDaysResultsURL = GenerateMatchDayResultsURL(CurrentMatchDay);
            List<HtmlNode> resultNodes = GetResultNodes(matchDaysResultsURL).GetAwaiter().GetResult();
            foreach (HtmlNode resultNode in resultNodes)
                NodeOperation(CurrentYear, CurrentDivision, CurrentMatchDay, resultNode, ResultsFilePath);
        }

        private string GenerateMatchDayResultsURL(int matchDay)
        {
            return string.Format("{0}/{1}/{2}/jornada_{3}/", ROOT_URL, CurrentDivision, Utils.YearToSeasonString(CurrentYear), matchDay);
        }

        //Adapted from https://stackoverflow.com/questions/43364856/get-web-page-using-htmlagilitypack-netcore
        private async Task<List<HtmlNode>> GetResultNodes(string matchDaysResultsURL)
        {
            HttpClient client = new HttpClient();
            using (HttpResponseMessage response = await client.GetAsync(matchDaysResultsURL))
            {
                using (HttpContent content = response.Content)
                {
                    byte[] contentBytes = await content.ReadAsByteArrayAsync();
                    string contentString = Encoding.GetEncoding(ENCODING).GetString(contentBytes);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(contentString);
                    if (!doc.DocumentNode.SelectSingleNode("//title").InnerText.Contains("404"))
                    {
                        HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//tr[@onclick]");
                        if (nodes != null)
                            return nodes.ToList();
                        else
                            return doc.DocumentNode.SelectNodes("//tr[@class]").Where(n => n.Attributes["class"].Value.Equals("nolink")).ToList();
                    }
                    else return new List<HtmlNode>();
                }
            }
        }

        public void ExecuteSingle(int year, string division, int matchDay)
        {
            CurrentYear = year;
            CurrentDivision = division;
            CurrentMatchDay = matchDay;
            ProcessCurrentMatchDay();
        }

    }
}
