using System.Linq;
using Mono.Cecil;

public class DataErrorInfoFinder
{
    public ValidationTemplateFinder ValidationTemplateFinder;
    public MethodDefinition TemplateConstructor;
    public bool Found = true;
    public MethodReference GetErrorMethod;
    public ModuleDefinition ModuleDefinition;
    public MethodReference GetItemMethod;
    public TypeReference InterfaceRef;
    public PropertyDefinition ErrorProperty;
    public PropertyDefinition ItemProperty;

    public void Execute()
    {
        var interfaces = ValidationTemplateFinder.TypeDefinition.Interfaces;
         InterfaceRef = interfaces.FirstOrDefault(x => x.Name == "IDataErrorInfo");
        if (InterfaceRef == null)
        {
            Found = false;
            return;
        }
        var interfaceType = InterfaceRef.Resolve();

        ErrorProperty = interfaceType.Properties.First(x => x.Name == "Error");
        ItemProperty = interfaceType.Properties.First(x => x.Name == "Item");
        GetErrorMethod = ModuleDefinition.Import(interfaceType.Methods.First(x => x.Name == "get_Error"));
        GetItemMethod = ModuleDefinition.Import(interfaceType.Methods.First(x => x.Name == "get_Item"));
    }

}