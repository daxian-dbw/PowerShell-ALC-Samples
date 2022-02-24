using System.Management.Automation;

namespace LowerDeps
{
    [Cmdlet("Test", "ConflictWithLowerDeps")]
    public class ConflictCommand : PSCmdlet
    {
        protected override void EndProcessing()
        {
            WriteObject(Shared.Dependency.GetNextGreeting());
        }
    }
}
