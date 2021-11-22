$null = New-Item -Type Directory -Name test-package
Set-Location ./test-package
if ($IsLinux -or $IsMacOS)
{
    cargo init
    cargo build
} 
else 
{
    cmd /c "cargo init 2>&1"
    cmd /c "cargo build 2>&1"
    if ($LASTEXITCODE) 
    {
        Write-Host "Something goes wrong"
        exit 1 
    }
}
