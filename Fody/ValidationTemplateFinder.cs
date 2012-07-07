using System.Linq;
using Mono.Cecil;

public class ValidationTemplateFinder
{
    public TypeDefinition TypeDefinition;
    public AllTypesFinder AllTypesFinder;

    public MethodDefinition TemplateConstructor;

    public void Execute()
    {
        TypeDefinition = AllTypesFinder.AllTypes.First(x => x.Name == "ValidationTemplate");
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