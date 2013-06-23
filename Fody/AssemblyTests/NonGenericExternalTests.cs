using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class NonGenericExternalTests
{
    string afterAssemblyPath;
    Assembly assembly;

    public NonGenericExternalTests()
    {
        AppDomainAssemblyFinder.Attach();
        var beforeAssemblyPath = Path.GetFullPath(@"..\..\..\WithNonGenericExternal\bin\Debug\WithNonGenericExternal.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void DataErrorInfo()
    {
        var instance = assembly.GetInstance("WithNonGenericExternal.Model");
        ValidationTester.TestDataErrorInfo(instance);
    }
    [Test]
    public void DataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithNonGenericExternal.ModelWithImplementation");
        ValidationTester.TestDataErrorInfo(instance);
    }

#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(afterAssemblyPath);
    }
#endif

    [Test]
    public void NotifyDataErrorInfo()
    {
        var instance = assembly.GetInstance("WithNonGenericExternal.Model");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
    [Test]
    public void NotifyDataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithNonGenericExternal.ModelWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

}