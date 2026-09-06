param([string]$EditorData = 'C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Data')
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
if (-not (Test-Path Artifacts/Mono/AirsoftClubIntegration_Data/Managed/Airsoft.Battle.dll)) { throw 'Run MonoBuild first' }
New-Item -ItemType Directory -Force Artifacts/AotManaged | Out-Null
Copy-Item Artifacts/Mono/AirsoftClubIntegration_Data/Managed/*.dll Artifacts/AotManaged
Copy-Item (Join-Path $EditorData 'MonoBleedingEdge/lib/mono/unityaot-win32/*.dll') Artifacts/AotManaged -Force
& (Join-Path $EditorData 'il2cpp/build/deploy/il2cpp.exe') --convert-to-cpp --platform=WindowsDesktop --architecture=x64 --dotnetprofile=unityaot-win32 --directory=Artifacts/AotManaged --generatedcppdir=Artifacts/AotConversion --data-folder=Artifacts/AotData --enable-array-bounds-check --enable-divide-by-zero-check --jobs=4 *> Artifacts/AotConversion.log
if ($LASTEXITCODE -ne 0) { throw 'AOT conversion failed; see Artifacts/AotConversion.log' }
foreach($file in @('Airsoft.Battle.cpp','AirsoftClub.Unity.cpp')) {
 if(-not(Test-Path -LiteralPath (Join-Path 'Artifacts/AotConversion' $file))) { throw "Missing converted $file" }
}
Write-Output 'AOT CONVERSION PASSED; native Windows IL2CPP build/run not validated by this check.'
