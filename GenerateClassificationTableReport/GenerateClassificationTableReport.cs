using Quinix;
using Quinix.Data;
using Quinix.Model;
using System;
using System.Linq;

class Program
{
    public static void Main()
    {
        //Adjust this values to get the desired classification table.
        int seasonYear = 2016;
        string divisionName = "primera";
        string reportFilePath = @"..\..\..\output\classification_table.csv";

        QuinixDbContext context = new QuinixDbContext();
        Season season = context.Seasons.Single(s => s.StartYear == seasonYear);
        Division division = context.Divisions.Single(d => d.Name.Equals(divisionName));
        ClassificationTable table = new ClassificationTable(season, division, 1, 38, 38);
        table.GenerateClassificationTable();
        ReportGenerator reportGenerator = new ReportGenerator(table.History, reportFilePath);
        reportGenerator.GenerateReport(38);

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

}
