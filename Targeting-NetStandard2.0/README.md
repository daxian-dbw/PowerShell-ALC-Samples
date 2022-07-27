## Target netstandard2.0

Some modules are built targeting netstandard2.0,
so that a single build can work on both .NET (PowerShell 7+) and .NET Framework (Windows PowerShell 5.1).
Also, it's quite common for a module to depend on `Newtonsoft.Json`,
and it happens a lot that a module starts to have assembly conflicts with PowerShell because it depends on a higher version of the `Newtonsoft.Json` assembly.

The type `AssemblyLoadContext` is not available when targeting netstandard2.0,
so we will show how to use `AppDomain.AssemblyResolve` and `Assembly.LoadFile` to work around this issue.

### Two scenarios

Samples here are for 2 scenarios:
1. The dependency of `Newtonsoft.Json` is delay-loaded only when the module business code gets to run.
2. The dependency of `Newtonsoft.Json` is at the class-level, and will be loaded as soon as the assembly gets loaded.

Please see the sub-folders for each of these 2 scenarios.
