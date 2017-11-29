using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public static class InstructionListExtensions
{
    public static void BeforeLast(this Collection<Instruction> collection, params Instruction[] instructions)
    {
        var index = collection.Count - 1;
        foreach (var instruction in instructions)
        {
            collection.Insert(index, instruction);
            index++;
        }
    }

    public static void Append(this Collection<Instruction> collection, params Instruction[] instructions)
    {
        for (var index = 0; index < instructions.Length; index++)
        {
            collection.Insert(index, instructions[index]);
        }
    }
}