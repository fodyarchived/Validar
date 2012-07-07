using System;
using Mono.Cecil;

public class ModuleWeaver
{
    AllTypesFinder allTypesFinder;
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
        allTypesFinder = new AllTypesFinder
                             {
                                 ModuleDefinition = ModuleDefinition
                             };
        allTypesFinder.Execute();

        templateFinder = new ValidationTemplateFinder
                             {
                                 AllTypesFinder = allTypesFinder
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
    }

    public void ProcessTypes()
    {
        foreach (var type in allTypesFinder.AllTypes)
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
        if (!typeDefinition.CustomAttributes.ContainsAttribute("InjectValidationAttribute"))
        {
            //TODO:log
            return;
        }

        if (dataErrorInfoFinder.Found)
        {
            var injector = new DataErrorInfoInjector
            {
                TypeDefinition = typeDefinition,
                TypeSystem = ModuleDefinition.TypeSystem,
                ValidationTemplateFinder = templateFinder,
                DataErrorInfoFinder = dataErrorInfoFinder,
                ModuleWeaver = this,
            };
            injector.Execute();
        }

        if (notifyDataErrorInfoFinder.Found)
        {
            var injector = new NotifyDataErrorInfoInjector
            {
                TypeDefinition = typeDefinition,
                NotifyDataErrorInfoFinder = notifyDataErrorInfoFinder,
                ValidationTemplateFinder = templateFinder,
                TypeSystem= ModuleDefinition.TypeSystem,
                ModuleWeaver = this,
            };
            injector.Execute();
        }
    }
}