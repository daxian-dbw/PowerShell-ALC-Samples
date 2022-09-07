#
# Module manifest for module 'SampleModule'
#

@{
    ModuleVersion = '0.0.1'
    GUID = 'bbcbf8b1-e3ea-408f-8823-1041810f6658'
    Author = 'dongbo'

    NestedModules = @('Greeting.Commands.dll')
    FunctionsToExport = @()
    CmdletsToExport = @('Get-Greeting')
    VariablesToExport = '*'
    AliasesToExport = @()
}
