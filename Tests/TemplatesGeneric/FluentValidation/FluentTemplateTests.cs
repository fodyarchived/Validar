﻿namespace TemplatesGeneric.FluentValidation
{
    using Xunit;
    using Xunit.Abstractions;

    public class TemplateTests :
        XunitLoggingBase
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