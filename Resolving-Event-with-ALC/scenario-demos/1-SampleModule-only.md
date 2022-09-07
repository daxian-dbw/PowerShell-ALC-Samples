## SampleModule only

> NOTE: This assumes you have built and generated the 3 modules successfully with `.\build.ps1`.

When `SampleModule` is the only module in the picture,
its resolving handler will serve the loading request of the `1.0.0.0` version of `SharedDependency.dll`.

```powershell
## PowerShell 7.2

PS:1> import-Module C:\arena\source\PowerShell-ALC-Samples\Resolving-Event-with-ALC\bin\SampleModule\SampleModule.psd1

PS:2> Get-Greeting -UseSharedDependency  ## triggers loading request of 'SharedDependency' from 'Greeting.Commands.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:3> Get-Greeting -UseSharedDependency  ## an assembly can trigger the loading of its reference assembly only once.
Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:4> Get-Greeting -UseLocalDependency   ## triggers loading request of 'SharedDependency' from 'LocalDependency.dll'.
<*** Fall in 'ResolvingHandler': SharedDependency, Version=1.0.0.0  -- Loaded! ***>
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'

PS:5> Get-Greeting -UseLocalDependency   ## again, 'LocalDependency.dll' can trigger the loading of 'SharedDependency' only once.
From <LocalDependency>: Greetings! -- from 'SharedDependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null', loaded in 'MyCustomALC'
```
