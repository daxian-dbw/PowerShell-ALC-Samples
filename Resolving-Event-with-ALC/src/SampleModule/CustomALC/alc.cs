using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Management.Automation;

namespace CustomAlc
{
    public class MyAlc : AssemblyLoadContext
    {
        private static MyAlc s_myAlc = new(
            Path.Combine(
                Path.GetDirectoryName(typeof(MyAlc).Assembly.Location),
                "Dependencies"));
        private string dependencyFolder;

        private MyAlc(string folder)
            : base("MyCustomALC", isCollectible: false)
        {
            dependencyFolder = folder;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string path = Path.Combine(dependencyFolder, assemblyName.Name) + ".dll";

            if (File.Exists(path))
            {
                return LoadFromAssemblyPath(path);
            }

            return null;
        }

        internal static Assembly ResolvingHandler(AssemblyLoadContext context, AssemblyName name)
        {
            if (name.FullName.Equals("SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>");
                return s_myAlc.LoadFromAssemblyName(name);
            }

            return null;
        }
    }

    public class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        public void OnImport()
        {
            AssemblyLoadContext.Default.Resolving += MyAlc.ResolvingHandler;
        }

        public void OnRemove(PSModuleInfo module)
        {
            AssemblyLoadContext.Default.Resolving -= MyAlc.ResolvingHandler;
        }
    }
}
