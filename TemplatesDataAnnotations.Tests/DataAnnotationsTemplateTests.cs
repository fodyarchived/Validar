using NUnit.Framework;

[TestFixture]
public class DataAnnotationsTemplateTests
{

    [Test]
    public void DataErrorInfo()
    {
        var instance = new Person();   
        ValidationTester.TestDataErrorInfo(instance);
    }

    [Test]
    public void NotifyDataErrorInfo()
    {
        var instance = new Person();   
        ValidationTester.TestNotifyDataErrorInfo(instance);
    }

}