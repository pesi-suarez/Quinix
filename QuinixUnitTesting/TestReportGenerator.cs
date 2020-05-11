using Xunit;
using System.Linq;
using Quinix.Data;
using Quinix.Model;
using Quinix;

namespace QuinixUnitTesting
{
    public class TestReportGenerator
    {
        [Fact]
        public void ExecuteSingle()
        {
            //NOTE: DB should be populated for season 2016-17 before executing the test.

            string resultsFilePath = @"..\..\..\test_data\TestReportGenerator\results.csv";
            string comparisonFilePath = @"..\..\..\test_data\TestReportGenerator\classification_comparison.csv";

            QuinixDbContext context = new QuinixDbContext();
            Season season = context.Seasons.Single(s => s.StartYear == 2016);
            Division division = context.Divisions.Single(d => d.Name.Equals("primera"));
            ClassificationTable table = new ClassificationTable(season, division, 1, 38, 38);
            table.GenerateClassificationTable();
            ReportGenerator reportGenerator = new ReportGenerator(table.History, resultsFilePath);
            reportGenerator.GenerateReport(38);

            Assert.Equal(Utils.GenerateFileHash(resultsFilePath), Utils.GenerateFileHash(comparisonFilePath));
        }

    }
}
