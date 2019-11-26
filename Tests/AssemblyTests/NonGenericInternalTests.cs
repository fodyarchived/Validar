using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class NonGenericInternalTests :
    VerifyBase
{
    static TestResult testResult;

    static NonGenericInternalTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("WithNonGenericInternal.dll",
            ignoreCodes: new[] { "0x80131869" });
    }

    [Fact]
    public void DataErrorInfo()
    {
        var instance = testResult.GetInstance("WithNonGenericInternal.Model");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void DataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithNonGenericInternal.ModelWithImplementation");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void NotifyDataErrorInfo()
    {
        var instance = testResult.GetInstance("WithNonGenericInternal.Model");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    [Fact]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithNonGenericInternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    public NonGenericInternalTests(ITestOutputHelper output) :
        base(output)
    {
    }
}