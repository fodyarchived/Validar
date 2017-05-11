using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class DataErrorInfoInjector
{
    public TypeDefinition TypeDefinition;
    public TypeSystem TypeSystem;
    public DataErrorInfoFinder DataErrorInfoFinder;
    public ModuleWeaver ModuleWeaver;
    MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
    public FieldDefinition ValidationTemplateField;

    public void Execute()
    {
        if (TypeDefinition.Interfaces.Any(x => x.InterfaceType.Name == DataErrorInfoFinder.InterfaceRef.Name))
        {
            var message = $"Skipped '{TypeDefinition.Name}' because it already implements '{DataErrorInfoFinder.InterfaceRef.Name}'.";
            ModuleWeaver.LogInfo(message);
            return;
        }
        AddInterface();
        AddGetError();
        AddGetItem();
    }

    void AddInterface()
    {
        TypeDefinition.Interfaces.Add(new InterfaceImplementation(DataErrorInfoFinder.InterfaceRef));
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
            Instruction.Create(OpCodes.Ldfld, ValidationTemplateField),
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
            Instruction.Create(OpCodes.Ldfld, ValidationTemplateField),
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


}