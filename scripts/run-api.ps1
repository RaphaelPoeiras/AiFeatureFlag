$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot\..

dotnet run --project .\src\AiFeatureFlags.Api\AiFeatureFlags.Api.csproj
