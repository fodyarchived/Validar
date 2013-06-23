using DataAnnotations;
using NUnit.Framework;

[TestFixture]
public class DataAnnotationsTemplateTests
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