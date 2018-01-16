using System.Linq;
using Mono.Cecil;

public class NotifyDataErrorInfoFinder
{
    public TypeReference InterfaceRef;
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

    public void Execute()
    {
        var interfaces = ValidationTemplateFinder.TypeDefinition.Interfaces;
        InterfaceRef = interfaces.Select(x => x.InterfaceType)
            .FirstOrDefault(x => x.Name == "INotifyDataErrorInfo");
        if (InterfaceRef == null)
        {
            Found = false;
            return;
        }
        var interfaceType = InterfaceRef.Resolve();
        InterfaceRef = ModuleDefinition.ImportReference(InterfaceRef);

        GetErrorsMethodDef = interfaceType.Methods.First(x => x.Name == "GetErrors");
        GetErrorsMethodRef = ModuleDefinition.ImportReference(GetErrorsMethodDef);
        ErrorsChangedEvent = interfaceType.Events.First(x => x.Name == "ErrorsChanged");
        ErrorsChangedEventType = ModuleDefinition.ImportReference(ErrorsChangedEvent.EventType);
        ErrorsChangedAddMethod = ModuleDefinition.ImportReference(ErrorsChangedEvent.AddMethod);
        ErrorsChangedRemoveMethod = ModuleDefinition.ImportReference(ErrorsChangedEvent.RemoveMethod);

        GetHasErrorsMethod = ModuleDefinition.ImportReference(interfaceType.Methods.First(x => x.Name == "get_HasErrors"));
    }
}