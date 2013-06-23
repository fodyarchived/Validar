using NUnit.Framework;
using Sandra;

[TestFixture]
public class SandraTemplateTests
{

    [Test]
    public void DataErrorInfo()
    {
        ValidationTester.TestDataErrorInfo(new Model());
    }

    [Test]
    public void NotifyDataErrorInfo()
    {
        ValidationTester.TestNotifyDataErrorInfo(new Model());
    }

}