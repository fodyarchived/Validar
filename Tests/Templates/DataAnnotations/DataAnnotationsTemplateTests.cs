namespace Templates.DataAnnotations
{
    using Xunit;

    public class TemplateTests
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
    }
}