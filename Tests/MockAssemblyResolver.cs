using System.IO;
using System.Reflection;
using Mono.Cecil;

public class MockAssemblyResolver :
    IAssemblyResolver
{
    public AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        var fileName = Path.Combine(Directory, name.Name) + ".dll";
        if (File.Exists(fileName))
        {
            return AssemblyDefinition.ReadAssembly(fileName);
        }
        try
        {
            var assembly = Assembly.Load(name.FullName);
            var codeBase = assembly.CodeBase.Replace("file:///", "");
            return AssemblyDefinition.ReadAssembly(codeBase);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {
        try
        {
            var codeBase = Assembly.Load(name.FullName).CodeBase.Replace("file:///", "");

            return AssemblyDefinition.ReadAssembly(codeBase);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }


    public string Directory;
    public void Dispose()
    {
    }
}