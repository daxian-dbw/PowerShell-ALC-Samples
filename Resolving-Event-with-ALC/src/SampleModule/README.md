## Module structure

The `SampleModule` has the following folder structure:
```
└───SampleModule
    │   Greeting.Commands.dll
    │   Greeting.Init.dll
    │   LocalDependency.dll
    │   SampleModule.psd1
    │
    └───Dependencies
            SharedDependency.dll
```

`SampleModule.psd1` declares 2 nested modules: `NestedModules = @('Greeting.Init.dll', 'Greeting.Commands.dll')`.

`Greeting.Init.dll` contains no business logic of the module, but only a custom implementation of `AssemblyLoadContext` and the code that registers a handler to `AssemblyLoadContext.Default.Resolving` event when the module is loaded (of course, unregister when the module is removed).

`Greeting.Commands.dll` contains the real business logic of the module and exposes the `Get-Greeting` cmdlet.
It references 2 dependencies: `SharedDependency.dll` and `LocalDependency.dll`.

`LocalDependency.dll` contains the real business logic of the module, which acts like a utility assembly needed by `Greeting.Commands.dll`.
It also references `SharedDependency.dll`.
This utility assembly is added to this sample intentionally, to demonstrate the behaviors when a module needs to request for loading the same assembly more than once.

`SharedDependency.dll` is the conflicting dependency assembly. The version referenced by `SampleModule` is `1.0.0.0`.

## How it works

The assembly `Greeting.Init.dll` is the first nested module to be loaded.
During the loading, its `OnImport` implementation will be called, which will register a handler to the `AssemblyLoadContext.Default.Resolving` event.

The handler only reacts to the loading request of the `1.0.0.0` version of `SharedDependency.dll`, becuase that's the version this module depends on.
The handler uses a singleton instance of the custom `AssemblyLoadContext` to serve all loading requests,
so the it's guaranteed to return the same assembly instance for all loading requests it serves.
The custom `AssemblyLoadContext` looks for the requested assembly from the `Dependencies` folder under the module base,
and that's why `SharedDependency.dll` is placed there.

The syntax of `Get-Greeting` is
```
Get-Greeting -UseSharedDependency [<CommonParameters>]

Get-Greeting -UseLocalDependency [<CommonParameters>]
```

When `-UseSharedDependency` is specified, the loading of `SharedDependency.dll` will be triggered.
When `-UseLocalDependency` is specified, the loading of `LocalDependency.dll` will be triggered,
which will then trigger another loading request of `SharedDependency.dll`.
