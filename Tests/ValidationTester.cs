using System.ComponentModel;
using System.Linq;
using NUnit.Framework;

public static class ValidationTester
{
    public static void TestDataErrorInfo(dynamic instance)
    {
        var dataErrorInfo = (IDataErrorInfo)instance;
        Assert.IsNotNullOrEmpty(dataErrorInfo.Error);
        Assert.AreEqual("'Given Names' should not be empty.", dataErrorInfo["GivenNames"]);
        Assert.AreEqual("'Family Name' should not be empty.", dataErrorInfo["FamilyName"]);
        instance.FamilyName = "foo";
        instance.GivenNames = "foo";
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
        Assert.AreEqual("'Given Names' should not be empty.", dataErrorInfo.GetErrors("GivenNames").Cast<string>().First());
        Assert.AreEqual("'Family Name' should not be empty.", dataErrorInfo.GetErrors("FamilyName").Cast<string>().First());
        instance.FamilyName = "foo";
        instance.GivenNames = "foo";
        Assert.IsFalse(dataErrorInfo.HasErrors);
        Assert.IsTrue(errorsChangedCalled);
        Assert.IsEmpty(dataErrorInfo.GetErrors("GivenNames").Cast<string>().ToList());
        Assert.IsEmpty(dataErrorInfo.GetErrors("FamilyName").Cast<string>().ToList());
    }

     
}