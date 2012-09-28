using System;
using System.Collections;
using System.ComponentModel;
using NotifyDataErrorInfo;

public class PersonTemplate : INotifyDataErrorInfo, INotifyPropertyChanged
{
    ValidationTemplate validationTemplate;

    public PersonTemplate()
    {
        validationTemplate = new ValidationTemplate(this);
    }
    
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }

    IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
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

    public event PropertyChangedEventHandler PropertyChanged;

}