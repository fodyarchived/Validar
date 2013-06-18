using System.IO;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
[Ignore]
public class ExperimentTest
{
    [Test]
    public void Simple()
    {
        var beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyWithNoValidationTemplate\bin\Debug\AssemblyWithNoValidationTemplate.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        var afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        var assembly = Assembly.LoadFile(afterAssemblyPath);
        var instance = assembly.GetInstance("AssemblyExperiments.ExperimentClass");
    }
}