using System;
using System.Collections;
using System.ComponentModel;
using DataErrorInfo;

public class PersonTemplate : IDataErrorInfo, INotifyPropertyChanged, INotifyDataErrorInfo
{
    ValidationTemplate validationTemplate;
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }

    public PersonTemplate()
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

    public IEnumerable GetErrors(string propertyName)
    {
        return ((INotifyDataErrorInfo)validationTemplate).GetErrors(propertyName);
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