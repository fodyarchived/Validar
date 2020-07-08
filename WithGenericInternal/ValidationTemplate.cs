using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace WithGenericInternal
{
    public class ValidationTemplate<T> :
        IDataErrorInfo,
        INotifyDataErrorInfo
        where T : INotifyPropertyChanged
    {
        IValidator validator;
        ValidationResult validationResult;
        ValidationContext<T> context;

        public ValidationTemplate(T target)
        {
            validator = ValidationFactory.GetValidator<T>();
            context = new ValidationContext<T>(target);
            validationResult = validator.Validate(context);
            target.PropertyChanged += Validate;
        }

        void Validate(object sender, PropertyChangedEventArgs e)
        {
            validationResult = validator.Validate(context);
            foreach (var error in validationResult.Errors)
            {
                RaiseErrorsChanged(error.PropertyName);
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return validationResult
                .Errors
                .Where(x => x.PropertyName == propertyName)
                .Select(x => x.ErrorMessage);
        }

        public bool HasErrors => validationResult.Errors.Count > 0;

        public string Error
        {
            get
            {
                var strings = validationResult
                    .Errors
                    .Select(x => x.ErrorMessage);
                return string.Join(Environment.NewLine, strings);
            }
        }

        public string this[string propertyName]
        {
            get
            {
                var strings = validationResult
                    .Errors
                    .Where(x => x.PropertyName == propertyName)
                    .Select(x => x.ErrorMessage);
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