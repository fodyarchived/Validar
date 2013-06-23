using System;
using System.Reflection;
using Scalpel;

[Remove]
public static class AppDomainAssemblyFinder
{

    public static void Attach()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (assembly.FullName == args.Name)
            {
                return assembly;
            }
        }
        return null;
    }
}