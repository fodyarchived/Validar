using System.Linq;
using Mono.Cecil;

public static class ImplementsInterface
{
    public static bool ImplementsINotify(this TypeReference typeReference)
    {
        if (typeReference is GenericParameter genericParameter)
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
        if (typeDefinition.Interfaces.Any(x => x.InterfaceType.Name == "INotifyPropertyChanged"))
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
        return typeReference.Constraints.Any(constraint => constraint.ImplementsINotify());
    }
}