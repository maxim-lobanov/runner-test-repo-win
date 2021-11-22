param (
    [string]$ExpectedVersion
)

if ([string]::IsNullOrEmpty($ExpectedVersion)) {
    Write-Host "Run tests for default Ruby. Skip this check"
    exit 0
}

$ActualVersion = & ruby --version

Write-Host "Expected version is '${ExpectedVersion}'"
Write-Host "Actual version is '${ActualVersion}'"

if ($ActualVersion -like "ruby ${ExpectedVersion}.*") {
    Write-Host "Toolcache version is correct"
    exit 0
} else {
    Write-Host "Toolcache version is incorrect"
    exit 1
}