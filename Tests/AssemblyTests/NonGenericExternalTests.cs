using System.Linq;
using Fody;
using Xunit;

public class NonGenericExternalTests
{
    static TestResult testResult;

    static NonGenericExternalTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("WithNonGenericExternal.dll",
            ignoreCodes:new []{ "0x80131869" });
    }

    [Fact]
    public void DataErrorInfo()
    {
        var instance = testResult.GetInstance("WithNonGenericExternal.Model");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void DataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithNonGenericExternal.ModelWithImplementation");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Fact]
    public void EnsureReferenceRemoved()
    {
        var instance = testResult.Assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "ValidationTemplateAttribute");
        Assert.Null(instance);
    }

    [Fact]
    public void NotifyDataErrorInfo()
    {
        var instance = testResult.GetInstance("WithNonGenericExternal.Model");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    [Fact]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = testResult.GetInstance("WithNonGenericExternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
}