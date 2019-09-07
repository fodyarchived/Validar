namespace Templates.DataAnnotations
{
    using Xunit;
    using Xunit.Abstractions;

    public class TemplateTests :
        XunitApprovalBase
    {
        [Fact]
        public void DataErrorInfo()
        {
            ValidationTester.TestDataErrorInfo(new Model());
        }

        [Fact]
        public void NotifyDataErrorInfo()
        {
            ValidationTester.TestNotifyDataErrorInfo(new Model());
        }

        public TemplateTests(ITestOutputHelper output) : 
            base(output)
        {
        }
    }
}