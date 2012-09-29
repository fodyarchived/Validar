#if (DEBUG)
using NUnit.Framework;

[TestFixture]
//TODO: sort the SL test
[Ignore]
public class SL5WeavingTaskTests : BaseTaskTests
{

    public SL5WeavingTaskTests()
        : base(@"AssemblyToProcess\AssemblyToProcessSilverlight5.csproj")
    {
    }


    [Test]
    public void NotifyDataErrorInfo()
    {
        var instance = Assembly.GetInstance("Person");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
    [Test]
    public void NotifyDataErrorInfoPersonWithImplementation()
    {
        var instance = Assembly.GetInstance("PersonWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
}
#endif