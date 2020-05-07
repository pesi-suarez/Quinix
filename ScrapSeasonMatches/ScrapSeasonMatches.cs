using Quinix;

public class Program
{
    static void Main(string[] args)
    {
        FootballMatchesWebScrapper scrapper = new FootballMatchesWebScrapper(2016, 2016, NodeOperation.SaveToDb, null);
        scrapper.ExecuteAll();
    }

}
