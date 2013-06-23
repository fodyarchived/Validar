using System.Linq;
using Mono.Cecil;

using Mono.Collections.Generic;

public static class CecilExtensions
{

    public static bool ContainsValidationAttribute(this Collection<CustomAttribute> attributes)
    {
        var firstOrDefault = attributes.FirstOrDefault(x => x.Constructor.DeclaringType.Name == "InjectValidationAttribute");
        if (firstOrDefault != null)
        {
            attributes.Remove(firstOrDefault);
            return true;
        }
        return false;
    }

    public static void ValidateIsOfType(this FieldReference targetReference, TypeReference expectedType)
    {
        if (targetReference.FieldType.FullName == expectedType.FullName)
        {
            return;
        }
        var message = string.Format("Field '{0}' could not be re-used because it is not the correct type. Expected '{1}'.", targetReference.FullName, expectedType.FullName);
        throw new WeavingException(message);
    }
}