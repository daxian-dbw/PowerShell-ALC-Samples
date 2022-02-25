## HigherConflict module loaded before SampleModule

> NOTE: this assumes you have built and generated the 3 modules successfully.

When the `HigherConflict` module gets loaded, the `1.5.0.0` version of `SharedDependency.dll` will be loaded into the default `AssemblyLoadContext`.
Then when loading and using `SampleModule`, the loading request for `1.0.0.0` version of `SharedDependency.dll` will be triggered,
which will be served directly by the default `AssemblyLoadContext` with the `1.5.0.0` version of `SharedDependency.dll` because the same or higher version of the requested assembly is already available.

```powershell
## PowerShell 7.2

PS:1> Import-Module C:\arena\source\PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin\HigherConflict\ConflictWithHigherDeps.dll
PS:2> gmo ConflictWithHigherDeps

ModuleType Version    PreRelease Name                                ExportedCommands
---------- -------    ---------- ----                                ----------------
Binary     1.0.0.0               ConflictWithHigherDeps              Test-ConflictWithHigherDeps

PS:3> Test-ConflictWithHigherDeps   ## the '1.5.0.0' version of 'SharedDependency' gets loaded in default ALC.
Greetings! -- from 'SharedDependency, Version=1.5.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'Default'

PS:4> import-Module C:\arena\source\PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin\SampleModule\SampleModule.psd1

PS:5> Get-Greeting -UseSharedDependency   ## the resolving handler is not triggered, because the request is served by '1.5.0.0' version of 'SharedDependency'.
Greetings! -- from 'SharedDependency, Version=1.5.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'Default'

PS:6> Get-Greeting -UseLocalDependency    ## same here, the loading request is served by '1.5.0.0' version of 'SharedDependency'.
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.5.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'Default'

## manually loading 'SharedDependency, Version=1.0.0.0' will be served by '1.5.0.0', not triggering the resolving handler.
PS:7> [System.Reflection.Assembly]::Load('SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null') | % FullName
SharedDependency, Version=1.5.0.0, Culture=neutral, PublicKeyToken=null
```
