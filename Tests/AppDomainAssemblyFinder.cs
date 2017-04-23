using System;
using System.Linq;
using System.Reflection;

public static class AppDomainAssemblyFinder
{
    static bool attached;
    public static void Attach()
    {
        if (!attached)
        {
            attached = true;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies.FirstOrDefault(assembly => assembly.FullName == args.Name);
    }
}