using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class GenericInternalTests
{
    string afterAssemblyPath;
    Assembly assembly;
    string beforeAssemblyPath;

    public GenericInternalTests()
    {
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WithGenericInternal.dll");
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void DataErrorInfo()
    {
        var instance = assembly.GetInstance("WithGenericInternal.Model");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Test]
    public void DataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithGenericInternal.ModelWithImplementation");
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
        var instance = assembly.GetInstance("WithGenericInternal.Model");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    [Test]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithGenericInternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
}