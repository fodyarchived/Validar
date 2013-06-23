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
            if (!type.ImplementsINotify())
            {
                LogInfo(string.Format("Skipping '{0}' because it does not implement INotifyPropertyChanged.", type.Name));
                continue;
            }
            ProcessType(type);
        }
    }
    public void ProcessType(TypeDefinition typeDefinition)
    {
        if (!typeDefinition.CustomAttributes.ContainsValidationAttribute())
        {
            return;
        }
        if (typeDefinition.HasGenericParameters)
        {
            throw new WeavingException(string.Format("Failed to process '{0}'. Generic models are not supported. Feel free to send a pull request.", typeDefinition.FullName));
        }
        var templateFieldInjector = new TemplateFieldInjector
                                                      {
                                                          ValidationTemplateFinder = templateFinder,
                                                          TargetType = typeDefinition,
                                                          TypeSystem = ModuleDefinition.TypeSystem
                                                      };
         templateFieldInjector.AddField();

        if (dataErrorInfoFinder.Found)
        {
            var injector = new DataErrorInfoInjector
            {
                TypeDefinition = typeDefinition,
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
                TypeDefinition = typeDefinition,
                NotifyDataErrorInfoFinder = notifyDataErrorInfoFinder,
                TypeSystem= ModuleDefinition.TypeSystem,
                ModuleWeaver = this,
                ValidationTemplateField = templateFieldInjector.ValidationTemplateField
            };
            injector.Execute();
        }
    }
}