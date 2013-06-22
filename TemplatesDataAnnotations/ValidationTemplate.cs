    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;


    public class ValidationTemplate : IDataErrorInfo, INotifyDataErrorInfo
    {
        ValidationContext validationContext;
        List<ValidationResult> validationResults;

        public ValidationTemplate(INotifyPropertyChanged target)
        {
            validationContext = GetValidator(target);
            validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, validationResults, true);
            target.PropertyChanged += Validate;
        }
        static ConcurrentDictionary<RuntimeTypeHandle, ValidationContext> validators = new ConcurrentDictionary<RuntimeTypeHandle, ValidationContext>();

        static ValidationContext GetValidator(object model)
        {
            ValidationContext validator;
            var typeHandle = model.GetType().TypeHandle;
            if (!validators.TryGetValue(typeHandle, out validator))
            {
                validators[typeHandle] = validator = new ValidationContext(model, null, null);
            }

            return validator;
        }

        void Validate(object sender, PropertyChangedEventArgs e)
        {
            validationResults.Clear();
            Validator.TryValidateObject(this, validationContext, validationResults, true);
            var hashSet = new HashSet<string>(validationResults.SelectMany(x => x.MemberNames));
            foreach (var error in hashSet)
            {
                RaiseErrorsChanged(error);
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return validationResults.Where(x => x.MemberNames.Contains(propertyName))
                                   .Select(x => x.ErrorMessage);
        }

        public bool HasErrors
        {
            get { return validationResults.Count > 0; }
        }

        public string Error
        {
            get
            {
                var strings = validationResults.Select(x => x.ErrorMessage)
                                              .ToArray();
                return string.Join(Environment.NewLine, strings);
            }
        }

        public string this[string propertyName]
        {
            get
            {
                var strings = validationResults.Where(x => x.MemberNames.Contains(propertyName))
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