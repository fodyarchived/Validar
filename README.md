## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

![Icon](https://raw.github.com/Fody/Validar/master/Icons/package_icon.png)

Provides validation for XAML binding models.

Injects [IDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.IDataErrorInfo.aspx) or [INotifyDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.INotifyDataErrorInfo.aspx) code into a class at compile time.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget package

http://nuget.org/packages/Validar.Fody 

### Your Model Code
(INotifyPropertyChanged implementation excluded for brevity)

    [InjectValidation]
    public class Person : INotifyPropertyChanged
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
    }

###Your validation template code

See [FluentValidationTemplate](https://github.com/Fody/Validar/wiki/FluentValidationTemplate) for an example ValidationTemplate implementation.


    public class ValidationTemplate : IDataErrorInfo
    {

        public ValidationTemplate(INotifyPropertyChanged target)
        {
            // Provide your own implementation
        }

        public string Error
        {
            get
            {
                // Provide your own implementation
            }
        }

        public string this[string propertyName]
        {
            get
            {
                // Provide your own implementation
            }
        }
    }


###What gets compiled

    public class Person : IDataErrorInfo , INotifyPropertyChanged
    {
        IDataErrorInfo validationTemplate;
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }

        public Person()
        {
            validationTemplate = new ValidationTemplate(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        string IDataErrorInfo.this[string columnName]
        {
            get { return validationTemplate[columnName]; }
        }

        string IDataErrorInfo.Error
        {
            get { return validationTemplate.Error; }
        }
    }


## Icon

<a href="http://thenounproject.com/noun/check-mark/#icon-No6407" target="_blank">Check Mark</a> designed by <a href="http://thenounproject.com/mateozlatar" target="_blank">Mateo Zlatar</a> from The Noun Project

