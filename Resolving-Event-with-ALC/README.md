# Resolving assembly conflicts

The article [Resolving PowerShell module assembly dependency conflicts][the-article] contains great content about what this problem is, why it happens, and different ways for a module author to mitigate the issue.
The [most robust solution][most-robust-solution] described in the article leverages the `AssemblyLoadContext` to handle the loading requests of all a module's dependencies,
which makes sure the module gets the exact version of the dependency assemblies that it requests for.

This techinique presents a clean solution for a module to avoid dependency conflicts.
It is used by the [Bicep PowerShell module](https://github.com/PSBicep/PSBicep),
and is also documented with a great example in this blog post: [Resolving PowerShell Module Conflicts](https://pipe.how/get-assemblyloadcontext/).

However, this techinique requires the module assembly to not directly reference the dependency assemblies,
but instead, to reference a wrapper assembly which then references the dependency assemblies.
The wrapper assembly acts like a bridge, forwarding the calls from the module assembly to the dependency assemblies.
This makes it usually a non-trivial amount of work to apply this techinique --
- For a new module, this would add additional complexity to the design and implementaion;
- For an existing module, this would require significant refactoring.

Here I want to introduce a simplified solution to mitigate the problem,
which comes with [two limitations](#limitations) comparing to the [above solution][most-robust-solution] but requires way less efforts from the module author.

## AssemblyLoadContext.Default.Resolving + AssemblyLoadContext

The use of the assembly resolving event is quite common for redirecting loading requests.
You can register an assembly resolving handler for the exact versions of your dependency assemblies,
and then leverage `AssemblyLoadContext` in the handler to deal with the loading.
With this, there is no need to have a warpper assembly,
and the handler is guaranteed to return the same assembly instance for all the loading requests it receives for the same assembly.

> NOTE: Do not use `Assembly.LoadFrom` in the event handler,
because that API always loads an assembly file to the default `AssemblyLoadContext`,
which is actually the source of this assembly-conflict problem.

There are two ways to leverage `AssemblyLoadContext` in the handler:
- Create a custom `AssemblyLoadContext` class and directly use it to load assembly files
- Use the `Assembly.LoadFile` API, which internally uses an anonymous `AssemblyLoadContext` to load an assembly file

`Assembly.LoadFile` is simpler to use,
but it loads different assembly files into different `AssemblyLoadContext` instances.
It's more recommended to create a custom `AssemblyLoadContext` class and use that for loading all dependencies,
especially when dependency assemblies may pull in other dependencies.

We have the module `SampleModule` to demonstrate this solution.
The whole sample is organized as follows:

- [shared-dependency](./src/shared-dependency/): it's a project to produce different versions of NuGet packages for `SharedDependency.dll`.
  Three such packages of the versions `0.7.0`, `1.0.0`, and `1.5.0` are available under the folder [nuget-packages](./nuget-packages/).
- `SampleModule`
  - [SampleModule-CustomALC](./src/SampleModule-CustomALC/): it produces the `SampleModule` that uses _"`Resolving` event + custom `AssemblyLoadContext`"_ to handle the conflicting `SharedDependency.dll`.
    See its [README](./src/SampleModule-CustomALC/README.md) for details on the module structure and how it works.
  - [SampleModule-LoadFile](./src/SampleModule-LoadFile/): it produces the `SampleModule` that uses _"`Resolving` event + `Assembly.LoadFile`"_ to handle the conflicting `SharedDependency.dll`.
    See its [README](./src/SampleModule-LoadFile/README.md) for details on the module structure and how it works.
- [ConflictWithHigherDeps](./src/HigherDependencyConflict/): it's a module that depends on a higher version of `SharedDependency.dll`
- [ConflictWithLowerDeps](./src/LowerDependencyConflict/): it's a module that depends on a lower version of `SharedDependency.dll`
- [scenario-demos](./scenario-demos/): it contains the demos for five scenarios that `SampleModule` can run into with the modules `ConflictWithHigherDeps` and `ConflictWithLowerDeps`.

To build and generate all the 3 modules needed for the demos,
run the following command within this folder:
```
## Specify 'CustomALC' to produces the 'SampleModule' module that uses a custom AssemblyLoadContext.
.\build.ps1 -UseTechnique CustomALC

## Or, specify 'LoadFile' to produces the 'SampleModule' module that uses 'Assembly.LoadFile'.
.\build.ps1 -UseTechnique LoadFile
```

The generated modules will be placed in `.\bin`.
Please make sure `.NET SDK 6` is installed and available in `PATH` before building.
The version of the SDK should be `6.0.100` or newer.

Once the 3 modules are generated under `.\bin`,
go ahead to [scenario-demos](./scenario-demos/) to review the behaviors of `SampleModule` for those five scenarios.

## Limitations

Comparing to technique adopted by the Bicep module, there are 2 limitations with this solution:
1. If a higher version of the dependency is already loaded in the default `AssemblyLoadContext`,
   that version will be used by your module, and the resolving handler will never be triggered.
1. If another module uses the same technique to handle the same version of the same dependency,
   and it's loaded before your module, then your module's request for that dependency will be served by that module's resolving handler.
   This's OK as long as that module is still loaded, but could potentially be a problem if that module is removed and unregistered the resolving handler that served your previous loading request.
   This is because if your module happens to have a new request for the same dependency after that point, the new request might then be served by your module's resolving handler with a new assembly instance, which could cause the type identity issue.

Please make sure you evaluate the limitations before going forward with this solution:
- For the 1st limitation, it may be acceptable to depend on a higher version dependency assembly at run time for some modules.
  For those modules, this solution could be a good fit.
- For the 2nd limitation, it would be rare to happen in practice, given that most workflows don't involve removing a loaded module.


[the-article]: https://docs.microsoft.com/powershell/scripting/dev-cross-plat/resolving-dependency-conflicts
[most-robust-solution]: https://docs.microsoft.com/powershell/scripting/dev-cross-plat/resolving-dependency-conflicts#loading-through-net-core-assembly-load-contexts
