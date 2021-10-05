# <img src="/package_icon.png" height="30px"> Validar.Fody

[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg)](https://gitter.im/Fody/Fody)
[![NuGet Status](https://img.shields.io/nuget/v/Validar.Fody.svg)](https://www.nuget.org/packages/Validar.Fody/)

Provides validation for XAML binding models.

Injects [IDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.IDataErrorInfo.aspx) or [INotifyDataErrorInfo](http://msdn.microsoft.com/en-us/library/system.componentmodel.INotifyDataErrorInfo.aspx) code into a class at compile time.


### This is an add-in for [Fody](https://github.com/Fody/Home/)

**It is expected that all developers using Fody either [become a Patron on OpenCollective](https://opencollective.com/fody/contribute/patron-3059), or have a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-fody?utm_source=nuget-fody&utm_medium=referral&utm_campaign=enterprise). [See Licensing/Patron FAQ](https://github.com/Fody/Home/blob/master/pages/licensing-patron-faq.md) for more information.**


## Usage

See also [Fody usage](https://github.com/Fody/Home/blob/master/pages/usage.md).


### NuGet installation

Install the [Validar.Fody NuGet package](https://nuget.org/packages/Validar.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package Validar.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<Validar/>` to [FodyWeavers.xml](https://github.com/Fody/Home/blob/master/pages/usage.md#add-fodyweaversxml)

```xml
<Weavers>
  <Validar/>
</Weavers>
```


## Model Code

 * Must implement `INotifyPropertyChanged` (in this case implementation excluded for brevity).
 * Contain a `[InjectValidation]` attribute.

For example

```c#
[InjectValidation]
public class Person : INotifyPropertyChanged
{
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }
}
```


### Validation template code

```c#
public class ValidationTemplate : IDataErrorInfo, INotifyDataErrorInfo
{
    public ValidationTemplate(INotifyPropertyChanged target)
    {
        // Provide an implementation
    }

    // implementation of IDataErrorInfo
    // implementation of INotifyDataErrorInfo
}
```


### What gets compiled

Note that an instance of `ValidationTemplate` has been injected into `Person` 

```c#
public class Person : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo 
{
    IDataErrorInfo validationTemplate;
    public string GivenNames { get; set; }
    public string FamilyName { get; set; }

    public Person()
    {
        validationTemplate = new ValidationTemplate(this);
    }

    // implementation of IDataErrorInfo
    // implementation of INotifyDataErrorInfo
}
```


## Validation Template

 * Must be named `ValidationTemplate`.
 * Namespace doesn't matter.
 * Must implement either `IDataErrorInfo` or `INotifyDataErrorInfo` or both.
 * Have a instance constructor that takes a `INotifyPropertyChanged`.
 * Can be generic e.g. `ValidationTemplate<T> where T: INotifyPropertyChanged`


### Current Assembly

If `ValidationTemplate`  exist in the current assembly they will be picked up automatically.


### Other Assembly

If `ValidationTemplate` exist in a different assembly add a `[ValidationTemplateAttribute]` to tell Validar where to look.

```c#
[assembly: ValidationTemplateAttribute(typeof(MyUtilsLibrary.ValidationTemplate))]
```


## Validation Template Implementations

Custom `ValidationTemplate` implementations are supported. Here are some suggested implementations that enable common validation libraries.


### [FluentValidation](https://github.com/JeremySkinner/FluentValidation)

```
Install-Package FluentValidation
```

Note that FluentValidation extracts the model validation into a different class

```c#
public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(x => x.FamilyName).NotEmpty();
        RuleFor(x => x.GivenNames).NotEmpty();
    }
}

public class ValidationTemplate<T> : IDataErrorInfo, INotifyDataErrorInfo where T : INotifyPropertyChanged
{
    T target;
    IValidator validator;
    ValidationResult validationResult;
    ValidationContext<T> context;
    static ConcurrentDictionary<RuntimeTypeHandle, IValidator> validators = new ConcurrentDictionary<RuntimeTypeHandle, IValidator>();

    public ValidationTemplate(T target)
    {
        this.target = target;
        validator = GetValidator(target.GetType());
        context = new ValidationContext<T>(target);
        validationResult = validator.Validate(context);
        target.PropertyChanged += Validate;
    }

    static IValidator GetValidator(Type modelType)
    {
        if (!validators.TryGetValue(modelType.TypeHandle, out var validator))
        {
            var typeName = string.Format("{0}.{1}Validator", modelType.Namespace, modelType.Name);
            var type = modelType.Assembly.GetType(typeName, true);
            validators[modelType.TypeHandle] = validator = (IValidator)Activator.CreateInstance(type);
        }
        return validator;
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
```


### [Sandra.SimpleValidator](https://github.com/phillip-haydon/Sandra.SimpleValidator)

```c#
Install-Package Sandra.SimpleValidator
```

Note that Sandra.SimpleValidator extracts the model validation into a different class

```c#
public class PersonValidator : ValidateThis<Person>
{
    public PersonValidator()
    {
        For(x => x.GivenNames)
            .Ensure(new Required().WithMessage("'Given Names' should not be empty."));

        For(x => x.FamilyName)
            .Ensure(new Required().WithMessage("'Family Name' should not be empty."));
    }
}

public class ValidationTemplate : IDataErrorInfo, INotifyDataErrorInfo
{
    INotifyPropertyChanged target;
    IModelValidator validator;
    ValidationResult validationResult;

    public ValidationTemplate(INotifyPropertyChanged target)
    {
        this.target = target;
        validator = GetValidator(target.GetType());
        validationResult = validator.Validate(target);
        target.PropertyChanged += Validate;
    }

    static ConcurrentDictionary<RuntimeTypeHandle, IModelValidator> validators = new ConcurrentDictionary<RuntimeTypeHandle, IModelValidator>();

    static IModelValidator GetValidator(Type modelType)
    {
        if (!validators.TryGetValue(modelType.TypeHandle, out var validator))
        {
            var typeName = string.Format("{0}.{1}Validator", modelType.Namespace, modelType.Name);
            var type = modelType.Assembly.GetType(typeName, true);
            validators[modelType.TypeHandle] = validator = (IModelValidator)Activator.CreateInstance(type);
        }
        return validator;
    }

    void Validate(object sender, PropertyChangedEventArgs e)
    {
        validationResult = validator.Validate(target);
        foreach (var error in validationResult.Messages)
        {
            RaiseErrorsChanged(error.PropertyName);
        }
    }

    public IEnumerable GetErrors(string propertyName)
    {
        return validationResult.Messages
                               .Where(x => x.PropertyName == propertyName)
                               .Select(x => x.Message);
    }

    public bool HasErrors
    {
        get { return validationResult.IsInvalid; }
    }

    public string Error
    {
        get
        {
            var strings = validationResult.Messages.Select(x => x.Message)
                                          .ToArray();
            return string.Join(Environment.NewLine, strings);
        }
    }

    public string this[string propertyName]
    {
        get
        {
            var strings = validationResult.Messages.Where(x => x.PropertyName == propertyName)
                                          .Select(x => x.Message)
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
```


### [DataAnnotations](http://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.aspx)

```c#
public class ValidationTemplate :
    IDataErrorInfo,
    INotifyDataErrorInfo
{
    INotifyPropertyChanged target;
    ValidationContext validationContext;
    List<ValidationResult> validationResults;

    public ValidationTemplate(INotifyPropertyChanged target)
    {
        this.target = target;
        validationContext = new ValidationContext(target, null, null);
        validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(target, validationContext, validationResults, true);
        target.PropertyChanged += Validate;
    }

    void Validate(object sender, PropertyChangedEventArgs e)
    {
        validationResults.Clear();
        Validator.TryValidateObject(target, validationContext, validationResults, true);
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
```


## Icon

[Check Mark](https://thenounproject.com/noun/check-mark/#icon-No6407) designed by [Mateo Zlatar](https://thenounproject.com/mateozlatar) from [The Noun Project](https://thenounproject.com).