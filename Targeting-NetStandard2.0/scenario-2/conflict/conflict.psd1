#
# Module manifest for module 'SampleModule'
#

@{
    ModuleVersion = '0.0.1'
    GUID = 'dd949fab-e6a8-4d02-b73f-39469bd1c9fa'
    Author = 'dongbo'

    NestedModules = @('resolver.dll', 'conflict.dll')
    FunctionsToExport = @()
    CmdletsToExport = @('Test-DummyCommand')
    VariablesToExport = '*'
    AliasesToExport = @()
}
