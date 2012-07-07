using System.Reflection;
using NUnit.Framework;

public abstract class BaseTaskTests
{
    string projectPath;
    public Assembly Assembly;

    protected BaseTaskTests(string projectPath)
    {

#if (!DEBUG)

            projectPath = projectPath.Replace("Debug", "Release");
#endif
        this.projectPath = projectPath;
    }

    [TestFixtureSetUp]
    public void Setup()
    {
        var weaverHelper = new WeaverHelper(projectPath);
        Assembly = weaverHelper.Assembly;
    }



#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(Assembly.CodeBase.Remove(0, 8));
    }
#endif

}