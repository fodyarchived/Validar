using System;
using System.Collections;
using System.ComponentModel;
using Scalpel;

namespace Fluent
{
    [Remove]
    public class Model : 
        IDataErrorInfo, 
        INotifyPropertyChanged, 
        INotifyDataErrorInfo
    {
        ValidationTemplate validationTemplate;
        public string Property1;
        public string Property2;

        public Model()
        {
            validationTemplate = new ValidationTemplate(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        string IDataErrorInfo.this[string columnName]
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

}