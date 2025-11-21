param(
  [switch] $Release
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$configuration = if ($Release) { 'Release' } else { 'Debug' }

Push-Location -Path 'Source'
try {
  dotnet build 'VirtuAwake.csproj' -c $configuration
}
finally {
  Pop-Location
}
