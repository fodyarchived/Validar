using NUnit.Framework;

namespace Templates.DataAnnotations
{
    [TestFixture]
    public class TemplateTests
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
}