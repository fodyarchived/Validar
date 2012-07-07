using NUnit.Framework;

[TestFixture]
public class Net4WeavingTaskTests : BaseTaskTests
{

    public Net4WeavingTaskTests()
        : base(@"AssemblyToProcess\AssemblyToProcessDotNet4.csproj")
    {

    }


    [Test]
    public void DataErrorInfo()
    {
        var instance = Assembly.GetInstance("Person");
        ValidationTester.TestDataErrorInfo(instance);
    }
    [Test]
    public void DataErrorInfoWithImplementation()
    {
        var instance = Assembly.GetInstance("PersonWithImplementation");
        ValidationTester.TestDataErrorInfo(instance);
    }

}