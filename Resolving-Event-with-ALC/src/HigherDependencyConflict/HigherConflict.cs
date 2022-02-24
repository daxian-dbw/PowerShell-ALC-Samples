using System.Management.Automation;

namespace HigherDeps
{
    [Cmdlet("Test", "ConflictWithHigherDeps")]
    public class ConflictCommand : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(Shared.Dependency.GetNextGreeting());
        }
    }
}
