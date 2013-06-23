using System.ComponentModel;
using System.Linq;
using NUnit.Framework;
using Scalpel;

[Remove]
public static class ValidationTester
{
    public static void TestDataErrorInfo(dynamic instance)
    {
        var dataErrorInfo = (IDataErrorInfo)instance;
        Assert.IsNotNullOrEmpty(dataErrorInfo.Error);
        Assert.AreEqual("'Property1' message.", dataErrorInfo["Property1"]);
        Assert.AreEqual("'Property2' message.", dataErrorInfo["Property2"]);
        instance.Property1 = "foo";
        instance.Property2 = "foo";
        Assert.IsEmpty(dataErrorInfo.Error);
        Assert.IsEmpty(dataErrorInfo["GivenNames"]);
        Assert.IsEmpty(dataErrorInfo["FamilyName"]);
    }
    public static void TestNotifyDataErrorInfo(dynamic instance)
    {
        var dataErrorInfo = (INotifyDataErrorInfo) instance;
        var errorsChangedCalled = false;
        dataErrorInfo.ErrorsChanged += (o, args) => { errorsChangedCalled = true; };
        Assert.IsTrue(dataErrorInfo.HasErrors);
        Assert.AreEqual("'Property1' message.", dataErrorInfo.GetErrors("Property1").Cast<string>().First());
        Assert.AreEqual("'Property2' message.", dataErrorInfo.GetErrors("Property2").Cast<string>().First());
        instance.Property1 = "foo";
        instance.Property2 = "foo";
        Assert.IsFalse(dataErrorInfo.HasErrors);
        Assert.IsTrue(errorsChangedCalled);
        Assert.IsEmpty(dataErrorInfo.GetErrors("GivenNames").Cast<string>().ToList());
        Assert.IsEmpty(dataErrorInfo.GetErrors("FamilyName").Cast<string>().ToList());
    }

     
}