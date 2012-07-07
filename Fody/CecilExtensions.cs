using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public static class CecilExtensions
{

    public static bool ContainsAttribute(this IEnumerable<CustomAttribute> attributes, string attributeName)
    {
        return attributes.Any(x => x.Constructor.DeclaringType.Name == attributeName);
    }

    public static void ValidateIsOfType(this FieldReference targetReference, TypeReference expectedType)
    {
        if (targetReference.FieldType.Name != expectedType.Name)
        {
            throw new WeavingException(string.Format("Field '{0}' could not be re-used because it is not the correct type. Expected '{1}'.", targetReference.Name, expectedType.Name));
        }
    }

}