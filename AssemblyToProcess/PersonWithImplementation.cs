using System;
using System.Collections;
using System.ComponentModel;
using Validar;

[InjectValidation]
public class PersonWithImplementation : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
{
    ValidationTemplate validationTemplate;
    public PersonWithImplementation()
    {
        validationTemplate = new ValidationTemplate(this);
    }

    public string this[string columnName]
    {
        get { return validationTemplate[columnName]; }
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

    public string Error
    {
        get { return validationTemplate.Error; }
    }

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

    void OnPropertyChanged(string propertyName)
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}