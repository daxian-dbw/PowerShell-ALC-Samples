#
# Module manifest for module 'SampleModule'
#

@{
    ModuleVersion = '0.0.1'
    GUID = '703c37b6-06c1-449a-9290-7a3316a53275'
    Author = 'dongbo'

    NestedModules = @('conflict.dll')
    FunctionsToExport = @()
    CmdletsToExport = @('Test-DummyCommand')
    VariablesToExport = '*'
    AliasesToExport = @()
}
