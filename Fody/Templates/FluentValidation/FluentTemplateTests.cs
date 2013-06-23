using Fluent;
using NUnit.Framework;

[TestFixture]
public class FluentTemplateTests
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