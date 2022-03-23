using System.ComponentModel;
using System.Linq;
using Xunit;

public static class ValidationTester
{
    public static void TestDataErrorInfo(dynamic instance)
    {
        var dataErrorInfo = (IDataErrorInfo)instance;
        Assert.NotNull(dataErrorInfo.Error);
        Assert.NotEmpty(dataErrorInfo.Error);
        Assert.Equal("'Property1' message.", dataErrorInfo["Property1"]);
        Assert.Equal("'Property2' message.", dataErrorInfo["Property2"]);
        instance.Property1 = "foo";
        instance.Property2 = "foo";
        Assert.Empty(dataErrorInfo.Error);
        Assert.Empty(dataErrorInfo["Property1"]);
        Assert.Empty(dataErrorInfo["Property2"]);
    }

    public static void TestNotifyDataErrorInfo(dynamic instance)
    {
        var dataErrorInfo = (INotifyDataErrorInfo)instance;
        var errorsChangedCalled = false;
        dataErrorInfo.ErrorsChanged += (o, args) => { errorsChangedCalled = true; };
        Assert.True(dataErrorInfo.HasErrors);
        Assert.Equal("'Property1' message.", dataErrorInfo.GetErrors("Property1").Cast<string>().First());
        Assert.Equal("'Property2' message.", dataErrorInfo.GetErrors("Property2").Cast<string>().First());
        instance.Property1 = "foo";
        instance.Property2 = "foo";
        Assert.False(dataErrorInfo.HasErrors);
        Assert.True(errorsChangedCalled);
        Assert.Empty(dataErrorInfo.GetErrors("Property1").Cast<string>().ToList());
        Assert.Empty(dataErrorInfo.GetErrors("Property2").Cast<string>().ToList());
    }
}