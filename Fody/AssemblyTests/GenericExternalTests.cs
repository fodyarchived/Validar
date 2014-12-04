using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class GenericExternalTests
{
    string afterAssemblyPath;
    Assembly assembly;

    public GenericExternalTests()
    {
        AppDomainAssemblyFinder.Attach();
        var beforeAssemblyPath = Path.GetFullPath(@"..\..\..\WithGenericExternal\bin\Debug\WithGenericExternal.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
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
        var instance = assembly.CustomAttributes.FirstOrDefault(x=>x.AttributeType.Name=="ValidationTemplateAttribute");
        Assert.IsNull(instance);
    }

    [Test]
    public void DataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("WithGenericExternal.ModelWithImplementation");
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