using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class GenericExternalTests
{
    string afterAssemblyPath;
    Assembly assembly;
    string beforeAssemblyPath;

    public GenericExternalTests()
    {
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WithGenericExternal.dll");
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void DataErrorInfo()
    {
        var instance = assembly.GetInstance("WithGenericExternal.MyModel");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Test]
    public void EnsureReferenceRemoved()
    {
        var instance = assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType.Name == "ValidationTemplateAttribute");
        Assert.IsNull(instance);
    }

    [Test]
    public void DataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithGenericExternal.ModelWithImplementation");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
    }

    [Test]
    public void NotifyDataErrorInfo()
    {
        var instance = assembly.GetInstance("WithGenericExternal.MyModel");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    [Test]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithGenericExternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
}