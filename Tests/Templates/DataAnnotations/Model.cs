using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Templates.DataAnnotations
{
    public class Model :
        IDataErrorInfo,
        INotifyPropertyChanged,
        INotifyDataErrorInfo
    {
        ValidationTemplate validationTemplate;
        string property1;
        string property2;

        [Required(ErrorMessage = "'Property1' message.")]        public string Property1        {
            get => property1;            set
            {
                property1 = value;
                OnPropertyChanged();
            }
        }

        [Required(ErrorMessage = "'Property2' message.")]        public string Property2        {
            get => property2;            set
            {
                property2 = value;
                OnPropertyChanged();
            }
        }

        public Model()
        {
            validationTemplate = new ValidationTemplate(this);
        }

        public string this[string columnName] => validationTemplate[columnName];

        public string Error => validationTemplate.Error;

        public IEnumerable GetErrors(string propertyName)
        {
            return validationTemplate.GetErrors(propertyName);
        }

        bool INotifyDataErrorInfo.HasErrors => validationTemplate.HasErrors;

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged        {
            add => validationTemplate.ErrorsChanged += value;            remove => validationTemplate.ErrorsChanged -= value;        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}