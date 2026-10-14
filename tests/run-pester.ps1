#requires -Version 5.1
$ErrorActionPreference = 'Stop'

$pester = Get-Module Pester -ListAvailable |
    Where-Object Version -GE ([version]'6.0.0') |
    Sort-Object Version -Descending |
    Select-Object -First 1
if (-not $pester) {
    throw 'Pester 6+ not found'
}

Import-Module $pester.Path -Force
$config = New-PesterConfiguration
$config.Run.Path = (Join-Path $PSScriptRoot 'pester')
$config.Run.PassThru = $true
$config.Output.Verbosity = 'Detailed'
$result = Invoke-Pester -Configuration $config
if ($result.TotalCount -eq 0 -or $result.Result -ne 'Passed') {
    throw "Pester tests failed in PowerShell $($PSVersionTable.PSVersion) ($($result.FailedCount) failed, $($result.TotalCount) total)."
}
