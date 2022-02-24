using System;
using System.Runtime.Loader;

namespace Shared
{
    public class Dependency
    {
        public static string GetNextGreeting()
        {
            string asmFullName = typeof(Dependency).Assembly.FullName;
            string contextName = AssemblyLoadContext.GetLoadContext(typeof(Dependency).Assembly).Name;
            return $"Greetings! -- from '{asmFullName}', loaded in '{contextName}'";
        }
    }
}
