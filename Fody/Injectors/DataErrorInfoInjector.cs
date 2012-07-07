using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class DataErrorInfoInjector
{
    public TypeDefinition TypeDefinition;
    FieldDefinition validationTemplateField;
    public ValidationTemplateFinder ValidationTemplateFinder;
    public TypeSystem TypeSystem;
    public DataErrorInfoFinder DataErrorInfoFinder;
    public ModuleWeaver ModuleWeaver;
    MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;

    public void Execute()
    {
        if (TypeDefinition.Interfaces.Any(x => x.Name == DataErrorInfoFinder.InterfaceRef.Name))
        {
            var message = string.Format("Skipped '{0}' because it already implements '{1}'.", TypeDefinition.Name, DataErrorInfoFinder.InterfaceRef.Name);
            ModuleWeaver.LogInfo(message);
            return;
        }
        AddInterface();
        AddField();
        AddConstructor();
        AddGetError();
        AddGetItem();
    }

    void AddInterface()
    {
        TypeDefinition.Interfaces.Add(DataErrorInfoFinder.InterfaceRef);
    }

    void AddGetError()
    {
        var method = new MethodDefinition(DataErrorInfoFinder.InterfaceRef.FullName + ".get_Error", MethodAttributes, TypeSystem.String)
                         {
                             IsGetter = true,
                             IsPrivate = true
                         };
        method.Overrides.Add(DataErrorInfoFinder.GetErrorMethod);
        method.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, validationTemplateField),
            Instruction.Create(OpCodes.Callvirt, DataErrorInfoFinder.GetErrorMethod),
            Instruction.Create(OpCodes.Ret)
            );
        var property = new PropertyDefinition(DataErrorInfoFinder.InterfaceRef.FullName + ".Error", PropertyAttributes.None, TypeSystem.String)
                           {
                               GetMethod = method
                           };
        TypeDefinition.Methods.Add(method);
        TypeDefinition.Properties.Add(property);
    }

    void AddGetItem()
    {
        var method = new MethodDefinition(DataErrorInfoFinder.InterfaceRef.FullName + ".get_Item", MethodAttributes, TypeSystem.String)
                             {
                                 IsGetter = true,
                                 IsPrivate = true,
                                 SemanticsAttributes = MethodSemanticsAttributes.Getter,
                             };
        method.Overrides.Add(DataErrorInfoFinder.GetItemMethod);
        method.Parameters.Add(new ParameterDefinition(TypeSystem.String));

        method.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, validationTemplateField),
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Callvirt, DataErrorInfoFinder.GetItemMethod),
            Instruction.Create(OpCodes.Ret));
        var property = new PropertyDefinition(DataErrorInfoFinder.InterfaceRef.FullName + ".Item", PropertyAttributes.None, TypeSystem.String)
                           {
                               GetMethod = method,
                           };
        TypeDefinition.Methods.Add(method);

        TypeDefinition.Properties.Add(property);
    }

    public void AddConstructor()
    {
        foreach (var constructor in TypeDefinition.GetConstructors().Where(c => !c.IsStatic))
        {
            ProcessConstructor(ValidationTemplateFinder.TemplateConstructor, constructor);
        }
    }

    public void ProcessConstructor(MethodDefinition templateConstructor, MethodDefinition constructor)
    {
        var body = constructor.Body;
        body.SimplifyMacros();
        body.MakeLastStatementReturn();
        body.Instructions.BeforeLast(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Newobj, templateConstructor),
            Instruction.Create(OpCodes.Stfld, validationTemplateField)
            );
        body.OptimizeMacros();
    }

    public void AddField()
    {
        validationTemplateField = TemplateFieldInjector.AddField(DataErrorInfoFinder.InterfaceRef, TypeDefinition);
    }
}