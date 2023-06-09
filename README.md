## Samples for isolating dependencies in the PowerShell module

This repository includes sample code to show the techniques that can be used
to isolate dependency assemblies for a PowerShell module,
so as to avoid facing conflicts in the assembly resolution when loading the module.

Currently, two overall scenarios are discussed:

1. [Resolving-Event-with-ALC](./Resolving-Event-with-ALC) -- for PowerShell modules that targets .NET 3.1 and above,
where the type `AssemblyLoadContext` is available.

2. [Targeting-NetStandard2.0](./Targeting-NetStandard2.0) -- for PowerShell modules that targets .NET Standard 2.0,
where the type `AssemblyLoadContext` is NOT available.

Check out the `README.md` in each of these 2 folders to see the detailed information.
