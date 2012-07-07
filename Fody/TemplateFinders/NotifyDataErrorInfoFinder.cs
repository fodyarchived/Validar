using System.Linq;
using Mono.Cecil;

public class NotifyDataErrorInfoFinder
{
    public TypeReference InterfaceRef;
    public AllTypesFinder AllTypesFinder;
    public bool Found = true;
    public MethodReference ErrorsChangedAddMethod;
    public MethodReference ErrorsChangedRemoveMethod;
    public EventDefinition ErrorsChangedEvent;
    public MethodReference GetErrorsMethodRef;
    public ValidationTemplateFinder ValidationTemplateFinder;
    public TypeReference ErrorsChangedEventType;
    public ModuleDefinition ModuleDefinition;
    public MethodDefinition GetErrorsMethodDef;
    public MethodReference GetHasErrorsMethod;
    public PropertyDefinition HasErrorsProperty;

    public void Execute()
    {
        var interfaces = ValidationTemplateFinder.TypeDefinition.Interfaces;
        InterfaceRef = interfaces.FirstOrDefault(x => x.Name == "INotifyDataErrorInfo");
        if (InterfaceRef == null)
        {
            Found = false;
            return;
        }
        var interfaceType = InterfaceRef.Resolve();

        GetErrorsMethodDef = interfaceType.Methods.First(x => x.Name == "GetErrors");
        GetErrorsMethodRef = ModuleDefinition.Import(GetErrorsMethodDef);
        ErrorsChangedEvent = interfaceType.Events.First(x => x.Name == "ErrorsChanged");
        ErrorsChangedEventType = ModuleDefinition.Import(ErrorsChangedEvent.EventType);
//        NotifyDataErrorInfoFinder.ErrorsChangedAddMethod.Parameters.First().ParameterType
        ErrorsChangedAddMethod = ModuleDefinition.Import(ErrorsChangedEvent.AddMethod);
        ErrorsChangedRemoveMethod = ModuleDefinition.Import(ErrorsChangedEvent.RemoveMethod);

        HasErrorsProperty = interfaceType.Properties.First(x => x.Name == "HasErrors");
        GetHasErrorsMethod = ModuleDefinition.Import(interfaceType.Methods.First(x => x.Name == "get_HasErrors"));
    }

}