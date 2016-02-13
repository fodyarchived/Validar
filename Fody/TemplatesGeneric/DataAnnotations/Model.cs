using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Scalpel;

namespace TemplatesGeneric.DataAnnotations
{
    [Remove]
    public class Model : 
        IDataErrorInfo, 
        INotifyPropertyChanged, 
        INotifyDataErrorInfo
    {
        ValidationTemplate<Model> validationTemplate;
        [Required(ErrorMessage = "'Property1' message.")] public string Property1;
        [Required(ErrorMessage = "'Property2' message.")] public string Property2;

        public Model()
        {
            validationTemplate = new ValidationTemplate<Model>(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string this[string columnName] => validationTemplate[columnName];

        public string Error => validationTemplate.Error;

        public IEnumerable GetErrors(string propertyName)
        {
            return validationTemplate.GetErrors(propertyName);
        }

        bool INotifyDataErrorInfo.HasErrors => validationTemplate.HasErrors;

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { validationTemplate.ErrorsChanged += value; }
            remove { validationTemplate.ErrorsChanged -= value; }
        }
    }
}