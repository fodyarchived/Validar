using System;
using System.Linq;
using Mono.Cecil;

public class ValidationTemplateFinder
{
    public TypeReference TypeReference;
    public TypeDefinition TypeDefinition;
    public MethodReference TemplateConstructor;
    public Action<string> LogInfo;
    public ModuleDefinition ModuleDefinition;

    public void Execute()
    {
        var validationTemplateAttribute = ModuleDefinition
            .Assembly
            .CustomAttributes
            .FirstOrDefault(x=>x.AttributeType.Name == "ValidationTemplateAttribute");

        if (validationTemplateAttribute == null)
        {
            ImportInternal();
            return;
        }
        ImportExternal(validationTemplateAttribute);
    }

    void ImportExternal(CustomAttribute validationTemplateAttribute)
    {
        var typeReference = (TypeReference) validationTemplateAttribute.ConstructorArguments.First().Value;
        TypeDefinition = typeReference.Resolve();
        TypeReference = ModuleDefinition.ImportReference(TypeDefinition);

        TemplateConstructor = ModuleDefinition.ImportReference(FindConstructor(TypeDefinition));
        ModuleDefinition
            .Assembly
            .CustomAttributes.Remove(validationTemplateAttribute);
    }

    void ImportInternal()
    {
        LogInfo("Could not find a 'ValidationTemplateAttribute' on the current assembly. Going to search current assembly for 'ValidationTemplate'.");

        TypeDefinition = ModuleDefinition
            .GetTypes()
            .FirstOrDefault(x =>
                x.Name == "ValidationTemplate" ||
                x.Name == "ValidationTemplate`1");
        if (TypeDefinition == null)
        {
            throw new WeavingException("Could not find a type named 'ValidationTemplate'");
        }
        TypeReference = TypeDefinition;

        TemplateConstructor = FindConstructor(TypeDefinition);
    }

    MethodDefinition FindConstructor(TypeDefinition typeDefinition)
    {
        var templateConstructor = typeDefinition.Methods.FirstOrDefault(x => x.IsConstructor && x.Parameters.Count == 1 && x.Parameters.First().ParameterType.ImplementsINotify());
        if (templateConstructor == null)
        {
            throw new WeavingException("Found 'ValidationTemplate' but it did not have a constructor that takes 'INotifyPropertyChanged' as a parameter");
        }

        return templateConstructor;
    }
}