using System;
using System.Collections;
using System.ComponentModel;
using Generic;
using Validar;

namespace WithGenericExternal
{
    [InjectValidation]
    public class ModelWithImplementation : 
        INotifyPropertyChanged, 
        IDataErrorInfo, 
        INotifyDataErrorInfo
    {
        ValidationTemplate<ModelWithImplementation> validationTemplate;
        public ModelWithImplementation()
        {
            validationTemplate = new ValidationTemplate<ModelWithImplementation>(this);
        }

        public string this[string columnName] => validationTemplate[columnName];

        public IEnumerable GetErrors(string propertyName)
        {
            return validationTemplate.GetErrors(propertyName);
        }
        public bool HasErrors => validationTemplate.HasErrors;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { validationTemplate.ErrorsChanged += value; }
            remove { validationTemplate.ErrorsChanged -= value; }
        }

        public string Error => validationTemplate.Error;

        string property1;

        public string Property1
        {
            get { return property1; }
            set
            {
                if (value != property1)
                {
                    property1 = value;
                    OnPropertyChanged("Property1");
                }
            }
        }

        string property2;

        public string Property2
        {
            get { return property2; }
            set
            {
                if (value != property2)
                {
                    property2 = value;
                    OnPropertyChanged("Property2");
                }
            }
        }

        void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}