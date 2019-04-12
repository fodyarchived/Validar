using System.Linq;
using Fody;
using Xunit;
using Xunit.Abstractions;

public class GenericExternalTests :
    XunitLoggingBase
{
    static TestResult testResult;

    static GenericExternalTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("WithGenericExternal.dll",
            ignoreCodes: new[] { "0x80131869" });
    }

    [Fact]
    public void DataErrorInfo()
    {
        var instance = testResult.GetInstance("WithGenericExternal.MyModel");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void EnsureReferenceRemoved()
    {
        var instance = testResult.Assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "ValidationTemplateAttribute");
        Assert.Null(instance);
    }

    [Fact]
    public void DataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithGenericExternal.ModelWithImplementation");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void NotifyDataErrorInfo()
    {
        var instance = testResult.GetInstance("WithGenericExternal.MyModel");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    [Fact]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithGenericExternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    public GenericExternalTests(ITestOutputHelper output) : 
        base(output)
    {
    }
}