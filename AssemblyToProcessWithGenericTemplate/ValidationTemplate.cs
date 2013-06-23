
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

public class ValidationTemplate<T> :
    IDataErrorInfo,
    INotifyDataErrorInfo
    where T : INotifyPropertyChanged
{
    INotifyPropertyChanged target;
    IValidator validator;
    ValidationResult validationResult;
    static ConcurrentDictionary<RuntimeTypeHandle, IValidator> validators = new ConcurrentDictionary<RuntimeTypeHandle, IValidator>();

    public ValidationTemplate(T target)
    {
        this.target = target;
        validator = GetValidator(target.GetType());
        validationResult = validator.Validate(target);
        target.PropertyChanged += Validate;
    }

    static IValidator GetValidator(Type modelType)
    {
        IValidator validator;
        if (!validators.TryGetValue(modelType.TypeHandle, out validator))
        {
            var typeName = string.Format("{0}.{1}Validator", modelType.Namespace, modelType.Name);
            var type = modelType.Assembly.GetType(typeName, true);
            validators[modelType.TypeHandle] = validator = (IValidator)Activator.CreateInstance(type);
        }

        return validator;
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