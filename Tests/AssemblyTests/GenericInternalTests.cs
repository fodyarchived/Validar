using Fody;
using Xunit;
using Xunit.Abstractions;

public class GenericInternalTests :
    XunitLoggingBase
{
    static TestResult testResult;

    static GenericInternalTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("WithGenericInternal.dll",
            ignoreCodes: new[] { "0x80131869" });
    }

    [Fact]
    public void DataErrorInfo()
    {
        var instance = testResult.GetInstance("WithGenericInternal.Model");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void DataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithGenericInternal.ModelWithImplementation");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void NotifyDataErrorInfo()
    {
        var instance = testResult.GetInstance("WithGenericInternal.Model");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    [Fact]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithGenericInternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    public GenericInternalTests(ITestOutputHelper output) :
        base(output)
    {
    }
}