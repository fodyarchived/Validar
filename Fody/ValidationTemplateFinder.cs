using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class ValidationTemplateFinder
{
    public TypeDefinition TypeDefinition;
    
    public MethodDefinition TemplateConstructor;

    public List<TypeDefinition> AllTypes;

    public void Execute()
    {
        TypeDefinition = AllTypes.FirstOrDefault(x => x.Name == "ValidationTemplate");
        if (TypeDefinition== null)
        {
            throw new WeavingException("Could not find a type named ValidationTemplate");
        }

        TemplateConstructor = TypeDefinition
            .Methods
            .FirstOrDefault(x =>
                            x.IsConstructor &&
                            x.Parameters.Count == 1 &&
                            x.Parameters.First().ParameterType.Name == "INotifyPropertyChanged");
        if (TemplateConstructor == null)
        {
            throw new WeavingException("Found ValidationTemplate but it did not have a constructor that takes INotifyPropertyChanged as a parameter");
        }
    }

}