using System.Linq;
using Mono.Cecil;

public static class ImplementsInterface
{
    public static bool ImplementsINotify(this TypeReference typeReference)
    {
        var genericParameter = typeReference as GenericParameter;
        if (genericParameter != null)
        {
            return ImplementsINotify(genericParameter);
        }
        if (typeReference.Name == "Object")
        {
            return false;
        }
        if (typeReference.Name == "INotifyPropertyChanged")
        {
            return true;
        }
        var typeDefinition = typeReference.Resolve();
        if (typeDefinition.Interfaces.Any(x=>x.Name=="INotifyPropertyChanged"))
        {
            return true;
        }
        if (typeDefinition.BaseType == null)
        {
            return false;
        }
        return typeDefinition.BaseType.ImplementsINotify();
    }
    public static bool ImplementsINotify(this GenericParameter typeReference)
    {
        foreach (var constraint in typeReference.Constraints)
        {
            if (constraint.ImplementsINotify())
            {
                return true;
            }
        }
        return false;
    }
}