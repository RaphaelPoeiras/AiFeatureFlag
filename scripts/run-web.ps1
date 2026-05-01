$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot\..\frontend\feature-flags-web

if (-not (Test-Path .\node_modules)) {
  npm install
}

npm run dev
