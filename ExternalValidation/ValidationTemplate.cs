using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace Generic
{
    public class ValidationTemplate<T> :
        IDataErrorInfo,
        INotifyDataErrorInfo
            where T : INotifyPropertyChanged
    {
        IValidator validator;
        ValidationResult result;
        ValidationContext<T> context;

        public ValidationTemplate(T target)
        {
            validator = ValidationFactory.GetValidator<T>();
            context = new ValidationContext<T>(target);
            result = validator.Validate(context);
            target.PropertyChanged += Validate;
        }


        void Validate(object sender, PropertyChangedEventArgs e)
        {
            result = validator.Validate(context);
            foreach (var error in result.Errors)
            {
                RaiseErrorsChanged(error.PropertyName);
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return result.Errors
                                   .Where(x => x.PropertyName == propertyName)
                                   .Select(x => x.ErrorMessage);
        }

        public bool HasErrors => result.Errors.Count > 0;

        public string Error
        {
            get
            {
                var strings = result.Errors.Select(x => x.ErrorMessage)
                                              .ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }

        public string this[string propertyName]
        {
            get
            {
                var strings = result.Errors.Where(x => x.PropertyName == propertyName)
                                              .Select(x => x.ErrorMessage)
                                              .ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        void RaiseErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            handler?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}