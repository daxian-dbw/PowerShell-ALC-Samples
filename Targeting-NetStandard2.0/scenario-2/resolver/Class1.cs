using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace assembly.resolver
{
    public class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private static string dependencyFolder = Path.Combine(
            Path.GetDirectoryName(typeof(Init).Assembly.Location),
            "Dependencies");

        public void OnImport()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolvingHandler;
        }

        public void OnRemove(PSModuleInfo module)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolvingHandler;
        }

        internal static Assembly ResolvingHandler(object sender, ResolveEventArgs args)
        {
            string name = args.Name;
            if (name.Equals("Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"<*** Fall in 'ResolvingHandler': Newtonsoft.Json, Version=13.0.0.0  -- Loaded! ***>");

                string path = Path.Combine(dependencyFolder, name.Split(',')[0]) + ".dll";
                if (File.Exists(path))
                {
                    // When running in .NET, the 'LoadFile' API uses an anonymous AssemblyLoadContext to load the assembly file.
                    // But it maintains a cache to guarantee that the same assembly instance is returned for the same assembly file path.
                    // For details, see the .NET code at https://source.dot.net/#System.Private.CoreLib/Assembly.cs,239
                    return Assembly.LoadFile(path);
                }
            }

            return null;
        }
    }
}
