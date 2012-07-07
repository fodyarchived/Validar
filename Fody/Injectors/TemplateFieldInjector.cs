using System.Linq;
using Mono.Cecil;

public static class TemplateFieldInjector
{
    public static FieldDefinition AddField(TypeReference fieldType, TypeDefinition targetType)
    {
        var fieldDefinition = targetType.Fields.FirstOrDefault(x => x.Name == "validationTemplate");
        if (fieldDefinition != null)
        {
            fieldDefinition.ValidateIsOfType(fieldType);
            return fieldDefinition;
        }
        fieldDefinition = new FieldDefinition("validationTemplate", FieldAttributes.Private, fieldType);
        targetType.Fields.Add(fieldDefinition);
        return fieldDefinition;
    }

}