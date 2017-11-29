using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class NonGenericInternalTests
{
    string afterAssemblyPath;
    Assembly assembly;
    string beforeAssemblyPath;

    public NonGenericInternalTests()
    {
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WithNonGenericInternal.dll");
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void DataErrorInfo()
    {
        var instance = assembly.GetInstance("WithNonGenericInternal.Model");
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Test]
    public void DataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithNonGenericInternal.ModelWithImplementation");
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
        var instance = assembly.GetInstance("WithNonGenericInternal.Model");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

    [Test]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithNonGenericInternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
}