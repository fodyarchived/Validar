using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace NotifyDataErrorInfo
{

    public static class ValidationFactory
    {
        static Dictionary<RuntimeTypeHandle, IValidator> validators = new Dictionary<RuntimeTypeHandle, IValidator>();

        public static IValidator GetValidator(Type modelType)
        {
            IValidator validator;
            if (!validators.TryGetValue(modelType.TypeHandle, out validator))
            {
                var typeName = modelType.Name + "Validator";
                var type = Type.GetType(modelType.Namespace + "." + typeName, true);
                validator = (IValidator) Activator.CreateInstance(type);
            }

            return validator;
        }
    }

    public class ValidationTemplate : INotifyDataErrorInfo
    {
        INotifyPropertyChanged target;

        public object Target
        {
            get { return target; }
            set
            {
                target = (INotifyPropertyChanged) value;
                validator = ValidationFactory.GetValidator(target.GetType());
                validationResult = validator.Validate(Target);
                target.PropertyChanged += Validate;
            }
        }

        void Validate(object sender, PropertyChangedEventArgs e)
        {
            validator.
            var context = new ValidationContext(target, new PropertyChain(), new MemberNameValidatorSelector(new[] { e.PropertyName }));
            var validate = validator.Validate(context);
            validate.Errors.LastOrDefault();
        }


        public static ValidationResult Validate<T>(IValidator validator, T instance, string property)
        {
           
        }

 


        IValidator validator;
        ValidationResult validationResult;

     

        public IEnumerable GetErrors(string propertyName)
        {
            return GetErrorsForProperty(propertyName);
        }
         IEnumerable<string> GetErrorsForProperty(string propertyName)
        {
            return validationResult.Errors
                .Where(x => x.PropertyName == propertyName)
                .Select(x => x.ErrorMessage);
        }

        public bool HasErrors
        {
            get
            {
                return validationResult.Errors.Count>0;
            }
        }

        void RaiseErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    }
}