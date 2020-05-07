using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Quinix
{
    //TODONEW: Claro, la clase imprime la tabla de clasificación expandida para una única jornada. Esto NO es la matriz de características, que lo que contiene son ratios de los dos equipos que se enfrentan, y el resultado.
    public class ReportGenerator
    {
        private const string HEADER = "matchday;pos;team;G_played;G_points;G_won;G_tied;G_lost;G_GF;G_GA;G_WS;G_TS;G_LS;G_US;G_NWS;G_SS;G_NSS;G_CSH;G_CON;H_played;H_points;H_won;H_tied;H_lost;H_GF;H_GA;H_WS;H_TS;H_LS;H_US;H_NWS;H_SS;H_NSS;H_CSH;H_CON;A_played;A_points;A_won;A_tied;A_lost;A_GF;A_GA;A_WS;A_TS;A_LS;A_US;A_NWS;A_SS;A_NSS;A_CSH;A_CON";

        public List<ClassificationTableItem> History { get; set; }
        public string ReportPath { get; set; }

        public ReportGenerator(List<ClassificationTableItem> history, string reportFilePath)
        {
            History = history;
            ReportPath = reportFilePath;
        }

        public void GenerateReport(int matchDay)
        {
            IEnumerable<ClassificationTableItem> classification = History.Where(h => h.MatchDay == matchDay).OrderBy(h => h.Position);

            using (StreamWriter writer = File.CreateText(ReportPath))
            {
                writer.WriteLine(HEADER);
                foreach (ClassificationTableItem item in classification)
                    writer.WriteLine(item.ToCsvString());
            }
        }
    }

}
