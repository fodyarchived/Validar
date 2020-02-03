using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public class ModuleWeaver: BaseModuleWeaver
{
    NotifyDataErrorInfoFinder notifyDataErrorInfoFinder;
    DataErrorInfoFinder dataErrorInfoFinder;
    ValidationTemplateFinder templateFinder;

    public override void Execute()
    {
        templateFinder = new ValidationTemplateFinder
        {
            LogInfo = x => base.WriteInfo(x),
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
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        return Enumerable.Empty<string>();
    }

    public override bool ShouldCleanReference => true;

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
                throw new WeavingException($"Found [InjectValidationAttribute] on '{type.Name}' but it doesn't implement INotifyPropertyChanged so cannot inject.");
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
            ModuleDefinition = ModuleDefinition,
            TypeSystem = TypeSystem
        };
        templateFieldInjector.AddField();

        if (dataErrorInfoFinder.Found)
        {
            var injector = new DataErrorInfoInjector
            {
                TypeDefinition = type,
                TypeSystem = TypeSystem,
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
                TypeSystem = TypeSystem,
                ModuleWeaver = this,
                ValidationTemplateField = templateFieldInjector.ValidationTemplateField
            };
            injector.Execute();
        }
    }
}