namespace MyModule
{
    public class LocalDependency
    {
        public static string GetGreetingMessage()
        {
            string msg = Shared.Dependency.GetNextGreeting();
            return $"From <LocalDependency>: {msg}";
        }
    }
}
