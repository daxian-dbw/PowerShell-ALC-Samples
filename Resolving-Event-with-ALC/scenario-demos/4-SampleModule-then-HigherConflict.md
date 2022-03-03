## HigherConflict module loaded after SampleModule

> NOTE: This assumes you have built and generated the 3 modules successfully with `.\build.ps1 -UseTechnique CustomALC`.
>
> The demo will work mostly the same when you build with `-UseTechnique LoadFile`.
The only difference is the name of the `AssemblyLoadContext`,
as `Assembly.LoadFile` uses the assembly file path for the name of the `AssemblyLoadContext` instance it creates.

When the `SampleModule` gets loaded and used first,
the loading request for the `1.0.0.0` version of `SharedDependency.dll` will be served by its resolving handler.

Note that, the default `AssemblyLoadContext` will not cache the assembly instance of the `1.0.0.0` version of `SharedDependency.dll` returned from the resolving handler. However, it will "take a note" of it -- I saw the `1.0.0.0` version of `SharedDependency.dll` got served by a handler, so requests for the same assembly should be sent to the handlers, even if a higher version of the assembly becomes available later in the default `AssemblyLoadContext`.

When the `HigherConflict` module gets loaded later, the `1.5.0.0` version of `SharedDependency.dll` will be loaded into the default `AssemblyLoadContext`.
However, when the loading request for the `1.0.0.0` version of `SharedDependency.dll` gets triggered again by the `LocalDependency.dll`,
it will not be served by the `1.5.0.0` version of `SharedDependency.dll` that is already available in the default `AssemblyLoadContext`,
but instead, the loading request will be sent to the resolving handler.
So, `LocalDependency.dll` will get the `1.0.0.0` version `SharedDependency.dll` returned from the resolving handler.

```powershell
## PowerShell 7.2

PS:1> import-module C:\arena\source\PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin\SampleModule\SampleModule.psd1

PS:2> Get-Greeting -UseSharedDependency   ## triggers loading request of 'SharedDependency' from 'Greeting.Commands.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:3> Get-Greeting -UseSharedDependency   ## an assembly can trigger the loading of its reference assembly only once.
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:4> import-module C:\arena\source\PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin\HigherConflict\ConflictWithHigherDeps.dll

PS:5> Test-ConflictWithHigherDeps   ## this cmdlet has '1.5.0.0' version of 'SharedDependency' loaded in default ALC.
Greetings! -- from 'SharedDependency, Version=1.5.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'Default'

PS:6> Get-Greeting -UseLocalDependency   ## triggers loading request of 'SharedDependency' from 'LocalDependency.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:7> Get-Greeting -UseLocalDependency   ## again, 'LocalDependency.dll' can trigger the loading of 'SharedDependency' only once.
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

## manually loading 'SharedDependency, Version=1.0.0.0' will trigger the resolving handler again.
PS:8> [System.Reflection.Assembly]::Load('SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null') | % FullName
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
```
