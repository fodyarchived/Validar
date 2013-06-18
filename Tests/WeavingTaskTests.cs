using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class Net4WeavingTaskTests 
{
    string beforeAssemblyPath;
    string afterAssemblyPath;
    Assembly assembly;

    public Net4WeavingTaskTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void DataErrorInfo()
    {
        var instance = assembly.GetInstance("Person");
        ValidationTester.TestDataErrorInfo(instance);
    }
    [Test]
    public void DataErrorInfoWithImplementation()
    {
        var instance = assembly.GetInstance("PersonWithImplementation");
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
        var instance = assembly.GetInstance("Person");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }
    [Test]
    public void NotifyDataErrorInfoPersonWithImplementation()
    {
        var instance = assembly.GetInstance("PersonWithImplementation");
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

}