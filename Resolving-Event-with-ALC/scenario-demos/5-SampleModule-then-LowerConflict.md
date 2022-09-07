## LowerConflict module loaded after SampleModule

> NOTE: This assumes you have built and generated the 3 modules successfully with `.\build.ps1`.

When the `SampleModule` gets loaded and used first,
the loading request for the `1.0.0.0` version of `SharedDependency.dll` will be served by its resolving handler.

When the `LowerConflict` module gets loaded, the `0.7.0.0` version of `SharedDependency.dll` will be loaded into the default `AssemblyLoadContext`.
The subsequent loading request for the `1.0.0.0` version of `SharedDependency.dll` will continue to be served by the resolving handler

```PowerShell
## PowerShell 7.2

PS:1> import-module C:\arena\source\PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin\SampleModule\SampleModule.psd1

PS:2> get-Greeting -UseSharedDependency   ## triggers loading request of 'SharedDependency' from 'Greeting.Commands.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:3> get-Greeting -UseSharedDependency   ## an assembly can trigger the loading of its reference assembly only once.
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:4> import-module C:\arena\source\PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin\lowerConflict\ConflictWithLowerDeps.dll

PS:5> Test-ConflictWithLowerDeps   ## this cmdlet has '0.7.0.0' version of 'SharedDependency' loaded in default ALC.
Greetings! -- from 'SharedDependency, Version=0.7.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'Default'

PS:6> get-Greeting -UseLocalDependency   ## triggers loading request of 'SharedDependency' from 'LocalDependency.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:7> get-Greeting -UseLocalDependency   ## again, 'LocalDependency.dll' can trigger the loading of 'SharedDependency' only once.
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

## manually loading 'SharedDependency, Version=1.0.0.0' will trigger the resolving handler again.
PS:8> [System.Reflection.Assembly]::Load('SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null') | % FullName
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
```
