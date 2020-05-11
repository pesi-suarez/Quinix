using Xunit;
using Quinix;

namespace QuinixUnitTesting
{
    public class TestFeatureMatrixGenerator
    {
        [Fact]
        public void ExecuteSingle()
        {
            //NOTE: DB should be populated for season 2016-17 before executing the test.
            string resultsFilePath = @"..\..\..\test_data\TestFeatureMatrixGenerator\features.csv";
            string comparisonFilePath = @"..\..\..\test_data\TestFeatureMatrixGenerator\features_comparison.csv";
            FeatureMatrixGenerator featureMatrixGenerator = new FeatureMatrixGenerator(resultsFilePath);
            featureMatrixGenerator.ProcessSingle(2016, "primera");
            featureMatrixGenerator.PrintFeatureMatrix();
            Assert.Equal(Utils.GenerateFileHash(resultsFilePath), Utils.GenerateFileHash(comparisonFilePath));
        }

    }
}
