using System.Linq;
using Mono.Cecil;

public static class ImplementsInterface
{
    public static bool ImplementsINotify(this TypeReference typeReference)
    {
        if (typeReference.Name == "Object")
        {
            return false;
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
}