## LowerConflict module loaded before SampleModule

> NOTE: This assumes you have built and generated the 3 modules successfully with `.\build.ps1` and have `Set-Location PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin`.

When the `LowerConflict` module gets loaded, the `0.7.0.0` version of `SharedDependency.dll` will be loaded into the default `AssemblyLoadContext`.
Then when loading and using `SampleModule`, the loading request for `1.0.0.0` version of `SharedDependency.dll` will be triggered,
which cannot be served by the `0.7.0.0` version of `SharedDependency.dll` that is already available in the default `AssemblyLoadContext`
because the version is lower than the requested version.
So, the registered resolving handler will be triggered, to serve the loading request for the `1.0.0.0` version of `SharedDependency.dll`.

```powershell
## PowerShell 7.2

PS:1> Import-Module .\bin\lowerConflict\ConflictWithLowerDeps.dll

PS:2> Get-Module ConflictWithLowerDeps

ModuleType Version    PreRelease Name                                ExportedCommands
---------- -------    ---------- ----                                ----------------
Binary     1.0.0.0               ConflictWithLowerDeps               Test-ConflictWithLowerDeps

PS:3> Test-ConflictWithLowerDeps   ## the '0.7.0.0' version of 'SharedDependency' gets loaded in default ALC.
Greetings! -- from 'SharedDependency, Version=0.7.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'Default'

PS:4> Import-Module .\bin\SampleModule\SampleModule.psd1

PS:5> Get-Greeting -UseSharedDependency   ## triggers loading request of 'SharedDependency' from 'Greeting.Commands.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:6> Get-Greeting -UseSharedDependency   ## an assembly can trigger the loading of its reference assembly only once.
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:7> Get-Greeting -UseLocalDependency    ## triggers loading request of 'SharedDependency' from 'LocalDependency.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:8> Get-Greeting -UseLocalDependency    ## again, 'LocalDependency.dll' can trigger the loading of 'SharedDependency' only once.
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'
```
