using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class TemplateFieldInjector
{
    public ValidationTemplateFinder ValidationTemplateFinder;

    public FieldDefinition AddField(TypeDefinition targetType)
    {
        var fieldDefinition = targetType.Fields.FirstOrDefault(x => x.Name == "validationTemplate");
        if (fieldDefinition != null)
        {
            fieldDefinition.ValidateIsOfType(ValidationTemplateFinder.TypeReference);
            return fieldDefinition;
        }
        fieldDefinition = new FieldDefinition("validationTemplate", FieldAttributes.Private, ValidationTemplateFinder.TypeReference);
        targetType.Fields.Add(fieldDefinition);

        AddConstructor(fieldDefinition, targetType);

        return fieldDefinition;
    }

    void AddConstructor(FieldReference fieldDefinition, TypeDefinition targetType)
    {
        foreach (var constructor in targetType.GetConstructors().Where(c => !c.IsStatic))
        {
            ProcessConstructor(fieldDefinition, constructor);
        }
    }

    public void ProcessConstructor(FieldReference fieldDefinition, MethodDefinition constructor)
    {
        var body = constructor.Body;
        body.SimplifyMacros();
        body.MakeLastStatementReturn();
        body.Instructions.BeforeLast(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Newobj, ValidationTemplateFinder.TemplateConstructor),
            Instruction.Create(OpCodes.Stfld, fieldDefinition)
            );
        body.OptimizeMacros();
    }

}