using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    NotifyDataErrorInfoFinder notifyDataErrorInfoFinder;
    DataErrorInfoFinder dataErrorInfoFinder;
    ValidationTemplateFinder templateFinder;
    public Action<string> LogInfo { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
    }

    public void Execute()
    {
        
        templateFinder = new ValidationTemplateFinder
                             {
                                 LogInfo = LogInfo,
                                 ModuleDefinition = ModuleDefinition,
                             };
        templateFinder.Execute();


        dataErrorInfoFinder = new DataErrorInfoFinder
                                  {
                                      ValidationTemplateFinder = templateFinder,
                                      ModuleDefinition = ModuleDefinition,
                                  };
        dataErrorInfoFinder.Execute();

        notifyDataErrorInfoFinder = new NotifyDataErrorInfoFinder
                                        {
                                            ValidationTemplateFinder = templateFinder,
                                            ModuleDefinition = ModuleDefinition,
                                        };
        notifyDataErrorInfoFinder.Execute();


        if (!dataErrorInfoFinder.Found && !notifyDataErrorInfoFinder.Found)
        {
            throw new WeavingException("Found ValidationTemplate but it did not implement INotifyDataErrorInfo or IDataErrorInfo");
        }
        ProcessTypes();
        RemoveReference();
    }

    public void ProcessTypes()
    {
        foreach (var type in ModuleDefinition.GetTypes().Where(x => x.IsClass))
        {
            ProcessType(type);
        }
    }
    public void ProcessType(TypeDefinition type)
    {
        var containsValidationAttribute = type.CustomAttributes.ContainsValidationAttribute();
        if (!type.ImplementsINotify())
        {
            if (containsValidationAttribute)
            {
                throw new WeavingException($"Found [InjectValidationAttribute] on '{type.Name}' but it doesnt implement INotifyPropertyChanged so cannot inject.");
            }
            return;
        }
        if (!containsValidationAttribute)
        {
            return;
        }
        if (type.HasGenericParameters)
        {
            throw new WeavingException($"Failed to process '{type.FullName}'. Generic models are not supported. Feel free to send a pull request.");
        }
        var templateFieldInjector = new TemplateFieldInjector
                                                      {
                                                          ValidationTemplateFinder = templateFinder,
                                                          TargetType = type,
                                                          ModuleDefinition = ModuleDefinition
                                                      };
         templateFieldInjector.AddField();

        if (dataErrorInfoFinder.Found)
        {
            var injector = new DataErrorInfoInjector
            {
                TypeDefinition = type,
                TypeSystem = ModuleDefinition.TypeSystem,
                DataErrorInfoFinder = dataErrorInfoFinder,
                ModuleWeaver = this,
                ValidationTemplateField = templateFieldInjector.ValidationTemplateField
            };
            injector.Execute();
        }

        if (notifyDataErrorInfoFinder.Found)
        {
            var injector = new NotifyDataErrorInfoInjector
            {
                TypeDefinition = type,
                NotifyDataErrorInfoFinder = notifyDataErrorInfoFinder,
                TypeSystem= ModuleDefinition.TypeSystem,
                ModuleWeaver = this,
                ValidationTemplateField = templateFieldInjector.ValidationTemplateField
            };
            injector.Execute();
        }
    }
}