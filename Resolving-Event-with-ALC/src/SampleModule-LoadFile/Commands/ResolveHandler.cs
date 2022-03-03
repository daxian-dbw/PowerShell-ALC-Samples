using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Management.Automation;

namespace CustomAlc
{
    public class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static string dependencyFolder = Path.Combine(
            Path.GetDirectoryName(typeof(Init).Assembly.Location),
            "Dependencies");

        public void OnImport()
        {
            AssemblyLoadContext.Default.Resolving += ResolvingHandler;
        }

        public void OnRemove(PSModuleInfo module)
        {
            AssemblyLoadContext.Default.Resolving -= ResolvingHandler;
        }

        internal static Assembly ResolvingHandler(AssemblyLoadContext context, AssemblyName name)
        {
            if (name.FullName.Equals("SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>");

                string path = Path.Combine(dependencyFolder, name.Name) + ".dll";
                if (File.Exists(path))
                {
                    // The 'LoadFile' API uses an anonymous AssemblyLoadContext to load the assembly file.
                    // But it maintains a cache to guarantee that the same assembly instance is returned for the same assembly file path.
                    // For details, see the .NET code at https://source.dot.net/#System.Private.CoreLib/Assembly.cs,239
                    return Assembly.LoadFile(path);
                }
            }

            return null;
        }
    }
}
