using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using Newtonsoft.Json;

namespace assembly.conflict
{
    [Cmdlet("Test", "DummyCommand")]
    public class DummyCommand : PSCmdlet
    {
        protected override void EndProcessing()
        {
            string typeName = typeof(JsonConvert).FullName;
            Console.WriteLine($"Using '{typeName}' from '{GetAssemblyName()}'");
        }

        private string GetAssemblyName()
        {
            return typeof(JsonConvert).Assembly.FullName;
        }
    }

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
                string fileName = name.Substring(0, name.IndexOf(',')) + ".dll";
                string filePath = Path.Combine(dependencyFolder, fileName);

                if (File.Exists(filePath))
                {
                    // In .NET, the 'LoadFile' API uses an anonymous 'AssemblyLoadContext' instance to load the assembly file.
                    // But it maintains a cache to guarantee that the same assembly instance is returned for the same assembly file path.
                    // For details, see the .NET code at https://source.dot.net/#System.Private.CoreLib/Assembly.cs,239
                    Console.WriteLine($"<*** Fall in 'ResolvingHandler': Newtonsoft.Json, Version=13.0.0.0  -- Loaded! ***>");
                    return Assembly.LoadFile(filePath);
                }
            }

            return null;
        }
    }
}
