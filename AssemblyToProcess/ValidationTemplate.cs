using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace DataErrorInfo
{

    public class ValidationTemplate : IDataErrorInfo, INotifyDataErrorInfo
    {
        INotifyPropertyChanged target;
        IValidator validator;
        ValidationResult validationResult;

        public ValidationTemplate(INotifyPropertyChanged target)
        {
            this.target = target;
            validator = ValidationFactory.GetValidator(target.GetType());
            validationResult = validator.Validate(target);
            target.PropertyChanged += Validate;
        }

        void Validate(object sender, PropertyChangedEventArgs e)
        {
   
                validationResult = validator.Validate(target);
                foreach (var error in validationResult.Errors)
                {
                    RaiseErrorsChanged(error.PropertyName);
                }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return validationResult.Errors
                .Where(x => x.PropertyName == propertyName)
                .Select(x => x.ErrorMessage);
        }

        public bool HasErrors
        {
            get { return validationResult.Errors.Count > 0; }
        }
        public string Error
        {
            get
            {
                var strings = validationResult.Errors.Select(x => x.ErrorMessage)
                    .ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }

        public string this[string propertyName]
        {
            get
            {
                var strings = validationResult.Errors.Where(x => x.PropertyName == propertyName)
                    .Select(x => x.ErrorMessage)
                    .ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }  
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        void RaiseErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}