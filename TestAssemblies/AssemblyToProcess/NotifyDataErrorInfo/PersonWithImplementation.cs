using System;
using System.Collections;
using System.ComponentModel;
using NotifyDataErrorInfo;

[InjectValidationAttribute]
public class PersonWithImplementation : INotifyPropertyChanged, INotifyDataErrorInfo
{
    
    INotifyDataErrorInfo validationTemplate;

    public PersonWithImplementation()
    {
        validationTemplate = new ValidationTemplate(this);
    }
    public IEnumerable GetErrors(string propertyName)
    {
        return validationTemplate.GetErrors(propertyName);
    }

    public bool HasErrors
    {
        get { return validationTemplate.HasErrors; }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
    {
        add { validationTemplate.ErrorsChanged += value; }
        remove { validationTemplate.ErrorsChanged -= value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    string givenNames;
    public string GivenNames
    {
        get { return givenNames; }
        set
        {
            if (value != givenNames)
            {
                givenNames = value;
                OnPropertyChanged("GivenNames");
            }
        }
    }
    string familyName;
    public string FamilyName
    {
        get { return familyName; }
        set
        {
            if (value != familyName)
            {
                familyName = value;
                OnPropertyChanged("FamilyName");
            }
        }
    }

    public virtual void OnPropertyChanged(string propertyName)
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}