$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
dotnet build AirsoftClubGame.sln -c Release
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
dotnet run --project tests/Airsoft.Battle.Tests -c Release --no-build
if ($LASTEXITCODE -ne 0) { throw 'Tests failed' }
dotnet run --project tests/Airsoft.Battle.Tests -c Release --no-build -- --simulate 10000
if ($LASTEXITCODE -ne 0) { throw 'Simulation failed' }
dotnet format whitespace AirsoftClubGame.sln --verify-no-changes --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Whitespace format failed' }
$source = Get-ChildItem src/Airsoft.Battle/Runtime -File -Filter *.cs
$forbidden = $source | Select-String -Pattern '\b(float|double|MonoBehaviour|GameObject|Transform)\b|UnityEngine|System\.Random|Time\.deltaTime'
if ($forbidden) { throw "Forbidden core dependency/arithmetic: $forbidden" }
git -c core.whitespace=blank-at-eol,blank-at-eof,space-before-tab,cr-at-eol diff --check
if ($LASTEXITCODE -ne 0) { throw 'Git whitespace failed' }
Write-Output 'VERIFICATION PASSED'
