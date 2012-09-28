using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Mono.Cecil;

public class WeaverHelper
{
    string projectPath;
    string assemblyPath;
    public Assembly Assembly { get; set; }

    public WeaverHelper(string projectPath)
    {
        this.projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\", projectPath));

        GetAssemblyPath();

        var newAssembly = assemblyPath.Replace(".dll", "2.dll");
        var pdbFileName = Path.ChangeExtension(assemblyPath, "pdb");
        var newPdbFileName = Path.ChangeExtension(newAssembly, "pdb");
        File.Copy(assemblyPath, newAssembly, true);
        File.Copy(pdbFileName, newPdbFileName, true);

        var assemblyResolver = new TestAssemblyResolver(assemblyPath, this.projectPath);
        var moduleDefinition = ModuleDefinition.ReadModule(newAssembly, new ReaderParameters {AssemblyResolver = assemblyResolver});
        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition,
            AssemblyResolver = assemblyResolver
        };

        weavingTask.Execute();
            var writerParameters = new WriterParameters
                                       {
                                           WriteSymbols = true
                                       };
            moduleDefinition.Write(newAssembly,writerParameters);

        Assembly = Assembly.LoadFile(newAssembly);
    }

    void GetAssemblyPath()
    {
        assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), GetOutputPathValue(), GetAssemblyName() + ".dll");
    }

    string GetAssemblyName()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        return xDocument.Descendants("AssemblyName")
            .Select(x => x.Value)
            .First();
    }

    string GetOutputPathValue()
    {
        var xDocument = XDocument.Load(projectPath);
        xDocument.StripNamespace();

        var outputPathValue = (from propertyGroup in xDocument.Descendants("PropertyGroup")
                               let condition = ((string)propertyGroup.Attribute("Condition"))
                               where (condition != null) &&
                                     (condition.Trim() == "'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'")
                               from outputPath in propertyGroup.Descendants("OutputPath")
                               select outputPath.Value).First();
#if (!DEBUG)
            outputPathValue = outputPathValue.Replace("Debug", "Release");
#endif
        return outputPathValue;
    }


}