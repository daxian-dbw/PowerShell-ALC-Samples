## Target netstandard2.0

Some modules are built targeting `netstandard2.0` (or `net462`),
so that a single build can work on both .NET (PowerShell 7+) and .NET Framework (Windows PowerShell 5.1).
Also, it's quite common for a module to depend on `Newtonsoft.Json`,
and it happens a lot that a module starts to have assembly conflicts with PowerShell because it depends on a higher version of the `Newtonsoft.Json` assembly.

The type `AssemblyLoadContext` is not available when targeting `netstandard2.0` or `net462`,
but it's easy to wrap a few reflection API calls to create a custom `AssemblyLoadContext` and load an assembly into it from path
when the module runs in the .NET (PowerShell 7+) environment.
So, we will create the `AssemblyLoadContextProxy` type that encapsulate those reflection operations,
and we will show how to use `AppDomain.AssemblyResolve` and `AssemblyLoadContextProxy` to work around this assembly conflict issue.

> **NOTE:** Do not use `Assembly.LoadFile` for the dependency isolation purpose.</br>
> This API does load an assembly to a separate `AssemblyLoadContext` instance, but assemblies loaded by
> this API are discoverable by PowerShell's type resolution code (see code [here](https://github.com/PowerShell/PowerShell/blob/918bb8c952af1d461abfc98bc709a1d359168a1c/src/System.Management.Automation/utils/ClrFacade.cs#L56-L61)).
> So, your module could run into the "_Type Identity_" issue when loading an assembly by `Assembly.LoadFile` while another module
> loads a different version of the same assembly into the default `AssemblyLoadContext`.

### Two scenarios

Samples here are for 2 scenarios:

1. The dependency of `Newtonsoft.Json` is delay-loaded only when the module business code gets to run.
2. The dependency of `Newtonsoft.Json` is at the class-level, and will be loaded as soon as the assembly gets loaded.

Please see the sub-folders for each of these 2 scenarios.
