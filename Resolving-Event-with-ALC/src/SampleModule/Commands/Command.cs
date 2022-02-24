using System.Management.Automation;

namespace MyModule
{
    [Cmdlet("Get", "Greeting")]
    public class MyCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "UseSharedDependency")]
        public SwitchParameter UseSharedDependency { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "UseLocalDependency")]
        public SwitchParameter UseLocalDependency { get; set; }

        protected override void EndProcessing()
        {
            switch (ParameterSetName)
            {
                case "UseSharedDependency":
                    WriteObject(GetMessageFromSharedDependency());
                    break;
                case "UseLocalDependency":
                    WriteObject(GetMessageFromLocalDependency());
                    break;
                default:
                    throw new System.Exception("Unreachable code.");
            }
        }

        private string GetMessageFromSharedDependency()
        {
            return Shared.Dependency.GetNextGreeting();
        }

        private string GetMessageFromLocalDependency()
        {
            return LocalDependency.GetGreetingMessage();
        }
    }
}
