using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Person : IDataErrorInfo, INotifyPropertyChanged, INotifyDataErrorInfo
{
    ValidationTemplate validationTemplate;
    [Required]
    public string GivenNames;
     [Required]
    public string FamilyName;

    public Person()
    {
        validationTemplate = new ValidationTemplate(this);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string this[string columnName]
    {
        get { return validationTemplate[columnName]; }
    }

    public string Error
    {
        get { return validationTemplate.Error; }
    }

    public IEnumerable GetErrors(string propertyName)
    {
        return validationTemplate.GetErrors(propertyName);
    }

    bool INotifyDataErrorInfo.HasErrors
    {
        get { return validationTemplate.HasErrors; }
    }
    event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
    {
        add { validationTemplate.ErrorsChanged += value; }
        remove { validationTemplate.ErrorsChanged -= value; }
    }
}