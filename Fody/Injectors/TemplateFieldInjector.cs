using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class TemplateFieldInjector
{
    public ValidationTemplateFinder ValidationTemplateFinder;
    public FieldDefinition ValidationTemplateField;
    public TypeDefinition TargetType;

    public void AddField()
    {
        ValidationTemplateField = TargetType.Fields.FirstOrDefault(x => x.Name == "validationTemplate");
        var fieldType = ValidationTemplateFinder.TypeReference;
        if (fieldType.HasGenericParameters)
        {
            var genericInstanceType = new GenericInstanceType(fieldType);
            genericInstanceType.GenericArguments.Add(TargetType);
            fieldType = genericInstanceType;
        }
        if (ValidationTemplateField != null)
        {
            ValidationTemplateField.ValidateIsOfType(fieldType);
            return;
        }
        ValidationTemplateField = new FieldDefinition("validationTemplate", FieldAttributes.Private, fieldType);
        TargetType.Fields.Add(ValidationTemplateField);
        AddConstructor();
    }

    void AddConstructor()
    {
        foreach (var constructor in TargetType.GetConstructors().Where(c => !c.IsStatic))
        {
            ProcessConstructor(constructor);
        }
    }

    public void ProcessConstructor(MethodDefinition targetConstructor)
    {
        MethodReference templateConstructor;
        if (ValidationTemplateFinder.TypeReference.HasGenericParameters)
        {
             templateConstructor = ValidationTemplateFinder.TemplateConstructor.MakeGenericInstanceMethod(TargetType);
        }
        else
        {
            templateConstructor = ValidationTemplateFinder.TemplateConstructor;
        }

        var body = targetConstructor.Body;
        body.SimplifyMacros();
        body.MakeLastStatementReturn();
        body.Instructions.BeforeLast(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Newobj, templateConstructor),
            Instruction.Create(OpCodes.Stfld, ValidationTemplateField)
            );
        body.OptimizeMacros();
    }

}