using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class GenericInternalTests 
{
    string afterAssemblyPath;
    Assembly assembly;

    public GenericInternalTests()
    {
        AppDomainAssemblyFinder.Attach();
        var beforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\WithGenericInternal\bin\Debug\WithGenericInternal.dll"));
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
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