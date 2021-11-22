param(
  [Parameter(Mandatory=$false)][string] $Variable1 = "Test variable value",
  [string] $runScriptInSeparateScope
)

. $PSScriptRoot\fileWithFunction.ps1

try {
  $runScriptInSeparateScopeBool = [System.Convert]::ToBoolean($runScriptInSeparateScope) 
} catch [FormatException] {
  $result = $false
}

Write-Host "Variables defined in entrypoint file v1 = $Variable1"

MethodInOtherScript -runScriptInSeparateScope $runScriptInSeparateScopeBool
