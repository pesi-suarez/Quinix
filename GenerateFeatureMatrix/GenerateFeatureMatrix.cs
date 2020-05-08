using Quinix;
using System.Globalization;

class Program
{
    //Will generate the feature matrix associated to all the football games present in the database.
    //May take up to 5 hours to finish when considering all games from season 2001-02 until now
    public static void Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-us");
        FeatureMatrixGenerator featureMatrixGenerator = new FeatureMatrixGenerator(@"..\..\..\output\features.csv");
        featureMatrixGenerator.Execute();
        featureMatrixGenerator.PrintFeatureMatrix();
    }

}
