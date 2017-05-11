using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;


public class NotifyDataErrorInfoInjector
{
    public TypeDefinition TypeDefinition;
    public ModuleWeaver ModuleWeaver;
    public TypeSystem TypeSystem;
    public NotifyDataErrorInfoFinder NotifyDataErrorInfoFinder;
    MethodAttributes MethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;
    public FieldDefinition ValidationTemplateField;

    public void Execute()
    {
        if (TypeDefinition.Interfaces.Any(x => x.InterfaceType.Name == NotifyDataErrorInfoFinder.InterfaceRef.Name))
        {
            ModuleWeaver.LogInfo($"Skipped '{TypeDefinition.Name}' because it already implements '{NotifyDataErrorInfoFinder.InterfaceRef.Name}'.");
            return;
        }
        AddInterface();
        AddErrorsChanged();
        AddHasErrors();
        AddGetErrors();
    }

    void AddGetErrors()
    {
        var method = new MethodDefinition(NotifyDataErrorInfoFinder.InterfaceRef.FullName + ".GetErrors", MethodAttributes, NotifyDataErrorInfoFinder.GetErrorsMethodRef.ReturnType)
                         {
                             IsPrivate = true,
                         };
        method.Overrides.Add(NotifyDataErrorInfoFinder.GetErrorsMethodRef);

        method.Parameters.Add(new ParameterDefinition(TypeSystem.String));

        method.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, ValidationTemplateField),
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Callvirt, NotifyDataErrorInfoFinder.GetErrorsMethodRef),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(method);
    }

    void AddHasErrors()
    {
        var method = new MethodDefinition(NotifyDataErrorInfoFinder.InterfaceRef.FullName + ".get_HasErrors", MethodAttributes, TypeSystem.Boolean)
                         {
                             IsGetter = true,
                             IsPrivate = true,
                         };
        method.Overrides.Add(NotifyDataErrorInfoFinder.GetHasErrorsMethod);
        method.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, ValidationTemplateField),
            Instruction.Create(OpCodes.Callvirt, NotifyDataErrorInfoFinder.GetHasErrorsMethod),
            Instruction.Create(OpCodes.Ret)
            );
        var property = new PropertyDefinition(NotifyDataErrorInfoFinder.InterfaceRef.FullName + ".HasErrors", PropertyAttributes.None, TypeSystem.Boolean)
                           {
                               GetMethod = method,
                           };
        TypeDefinition.Methods.Add(method);
        TypeDefinition.Properties.Add(property);
    }


    void AddErrorsChanged()
    {
        var eventDefinition = new EventDefinition(NotifyDataErrorInfoFinder.InterfaceRef.FullName + ".ErrorsChanged", EventAttributes.SpecialName, NotifyDataErrorInfoFinder.ErrorsChangedEventType)
                                  {
                                      AddMethod = GetAdd(),
                                      RemoveMethod = GetRemove(),
                                  };

        TypeDefinition.Events.Add(eventDefinition);
    }

    MethodDefinition GetAdd()
    {
        var add = new MethodDefinition(NotifyDataErrorInfoFinder.InterfaceRef.FullName + ".add_ErrorsChanged", MethodAttributes, TypeSystem.Void)
                      {
                          SemanticsAttributes = MethodSemanticsAttributes.AddOn,
                          IsPrivate = true,
                      };
        add.Overrides.Add(NotifyDataErrorInfoFinder.ErrorsChangedAddMethod);
        add.Parameters.Add(new ParameterDefinition(NotifyDataErrorInfoFinder.ErrorsChangedEventType));
        add.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, ValidationTemplateField),
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Callvirt, NotifyDataErrorInfoFinder.ErrorsChangedAddMethod),
            Instruction.Create(OpCodes.Ret));
        TypeDefinition.Methods.Add(add);
        return add;
    }

    MethodDefinition GetRemove()
    {
        var remove = new MethodDefinition(NotifyDataErrorInfoFinder.InterfaceRef.FullName + ".remove_ErrorsChanged", MethodAttributes, TypeSystem.Void)
                         {
                             SemanticsAttributes = MethodSemanticsAttributes.RemoveOn,
                             IsPrivate = true
                         };
        remove.Overrides.Add(NotifyDataErrorInfoFinder.ErrorsChangedRemoveMethod);
        remove.Parameters.Add(new ParameterDefinition(NotifyDataErrorInfoFinder.ErrorsChangedEventType));
        remove.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, ValidationTemplateField),
            Instruction.Create(OpCodes.Ldarg_1),
            Instruction.Create(OpCodes.Callvirt, NotifyDataErrorInfoFinder.ErrorsChangedRemoveMethod),
            Instruction.Create(OpCodes.Ret));
        TypeDefinition.Methods.Add(remove);
        return remove;
    }

    void AddInterface()
    {
        TypeDefinition.Interfaces.Add(new InterfaceImplementation(NotifyDataErrorInfoFinder.InterfaceRef));
    }

}