function MethodInOtherScript()  { 

  param(
       [bool]
       $runScriptInSeparateScope
    )

  Write-Output "Inner function called"

  Write-Output "Call variables from Not Global scope"
  if (Get-Variable Variable1 -ErrorAction SilentlyContinue)
  { Write-Output "Test passed, the variable is accessible in not global scope" } else {
    Write-Error "Variable doesn't exists in not global scope" -ErrorAction Stop 
  }

  Write-Output "Call variables from Global scope"
  if (Get-Variable Variable1 -Scope Global -ErrorAction SilentlyContinue)
  {
    if ( $runScriptInSeparateScope )
    {
      Write-Error "Variable shouldn't exist in global scope" -ErrorAction Stop
    }
    else
    {
      Write-Output "Test passed, the variable is accessible from the global scope"
    }
  }
  else
  {
    if ( $runScriptInSeparateScope )
    {
      Write-Output "Test passed, the variable isn't accessible from the global scope"
    }
    else
    {
      Write-Error "Variable doesn't exists in global scope" -ErrorAction Stop
    }
  }
}
