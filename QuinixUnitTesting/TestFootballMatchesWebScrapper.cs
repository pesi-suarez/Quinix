using Xunit;
using Quinix;
using System.IO;
using System.Linq;
using Quinix.Data;

namespace QuinixUnitTesting
{
    public class TestFootballMatchesWebScrapper
    {
        [Fact]
        public void WebScrapperToFile2016_2017()
        {
            string resultsFilePath = @"..\..\..\test_data\TestFootballMatchesWebScrapper\test_results.txt";
            string comparisonFilePath = @"..\..\..\test_data\TestFootballMatchesWebScrapper\comparison_results.txt";
            if (File.Exists(resultsFilePath))
                File.Delete(resultsFilePath);
            FootballMatchesWebScrapper mrp = new FootballMatchesWebScrapper(2016, 2016, NodeOperation.WriteToFile, resultsFilePath);
            mrp.ExecuteAll();

            Assert.Equal(Utils.GenerateFileHash(resultsFilePath), Utils.GenerateFileHash(comparisonFilePath));
        }

        [Fact]
        public void WebScrapperToDb2016_2017()
        {
            //TODONEW: Probar a instalar y ejecutar el proyecto desde cero.
            //TODONEW: Eliminar o traducir los comentarios -> Los comentarios que quedan son relevantes. Hay que traducirlos.
            //NOTA: Debe usarse la BD de test (ver app.config)
            QuinixDbContext context = new QuinixDbContext();
            var matches = context.Matches;
            foreach (var match in matches)
            {
                context.Remove(match);
            }
            context.SaveChanges();
            Assert.Equal(0, context.Matches.Count());

            FootballMatchesWebScrapper mrp = new FootballMatchesWebScrapper(2016, 2016, NodeOperation.SaveToDb, null);
            mrp.ExecuteAll();

            //Deben haberse creado 841 registros.
            Assert.Equal(841, context.Matches.Count());

            //Si vuelvo a ejecutar, debe seguir habiendo el mismo número de registros.
            mrp.ExecuteAll();
            Assert.Equal(841, context.Matches.Count());
        }

    }
}
