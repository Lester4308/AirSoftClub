$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$root=(Get-Location).Path
$bundle=Join-Path $root 'Artifacts/DevelopmentBundle'
New-Item -ItemType Directory -Force $bundle | Out-Null
if(!(Test-Path 'Artifacts/IL2CPP/AirsoftClubIntegration.exe')){throw 'Build native client first'}
New-Item -ItemType Directory -Force (Join-Path $bundle 'Client') | Out-Null
Copy-Item -Path (Join-Path $root 'Artifacts/IL2CPP/*') -Destination (Join-Path $bundle 'Client') -Recurse -Force
Copy-Item -LiteralPath (Join-Path $root 'compose.yaml') -Destination $bundle -Force
Copy-Item -LiteralPath (Join-Path $root 'implementation/DEVELOPMENT_RUNBOOK.md') -Destination $bundle -Force
foreach($report in @('MONETIZATION_BALANCE_014.md','VISUAL_FOUNDATION_014.md','VISUAL_BETA_015.md')) { Copy-Item -LiteralPath (Join-Path $root ('implementation/'+$report)) -Destination $bundle -Force }
@{Commit=(git rev-parse HEAD);Client='Windows x64 IL2CPP Development';Unity='6000.3.21f1';ExecutableSha256=(Get-FileHash (Join-Path $bundle 'Client/AirsoftClubIntegration.exe')).Hash;GameAssemblySha256=(Get-FileHash (Join-Path $bundle 'Client/GameAssembly.dll')).Hash}|ConvertTo-Json|Set-Content (Join-Path $bundle 'BUILD-MANIFEST.json')
New-Item -ItemType Directory -Force (Join-Path $bundle 'evidence/014') | Out-Null
Copy-Item -Path (Join-Path $root 'implementation/evidence/014/*') -Destination (Join-Path $bundle 'evidence/014') -Recurse -Force
New-Item -ItemType Directory -Force (Join-Path $bundle 'evidence/015') | Out-Null
Copy-Item -Path (Join-Path $root 'implementation/evidence/015/*') -Destination (Join-Path $bundle 'evidence/015') -Recurse -Force
$launcher=@'
$ErrorActionPreference='Stop'
Set-Location $PSScriptRoot
$secretPath=Join-Path $PSScriptRoot 'local-db-password.txt'
if(!(Test-Path $secretPath)){[IO.File]::WriteAllText($secretPath,[Guid]::NewGuid().ToString('N'))}
$env:AIRSOFT_DB_PASSWORD=[IO.File]::ReadAllText($secretPath)
$env:AIRSOFT_CONNECTION="Host=127.0.0.1;Port=55433;Database=airsoft_club_dev;Username=airsoft_dev;Password=$env:AIRSOFT_DB_PASSWORD"
$env:ASPNETCORE_ENVIRONMENT='Development'
$env:AIRSOFT_DEV_AUTH='1'
$env:ASPNETCORE_URLS='http://127.0.0.1:5080'
docker compose -p airsoft-club-bundle up -d --wait
if($LASTEXITCODE -ne 0){throw 'DB start failed'}
dotnet Server/Airsoft.Server.dll --migrate
if($LASTEXITCODE -ne 0){throw 'Migration failed'}
Write-Output 'Backend at http://127.0.0.1:5080. Open Client/AirsoftClubIntegration.exe to play. Ctrl+C stops the backend.'
dotnet Server/Airsoft.Server.dll
'@
[IO.File]::WriteAllText((Join-Path $bundle 'Start-Backend.ps1'),$launcher)
$compose=[IO.File]::ReadAllText((Join-Path $bundle 'compose.yaml')).Replace('55470:5432','55433:5432').Replace('55432:5432','55433:5432')
[IO.File]::WriteAllText((Join-Path $bundle 'compose.yaml'),$compose)
$notice='Requires Windows x64, .NET10 runtime and Docker Desktop. Run Start-Backend.ps1, then run Start-Game.ps1 to open the current UGUI client. Stop another development backend using port5080 first. Database/password are local to this bundle. Prototype art; no live Steam/payments. See DEVELOPMENT_RUNBOOK.md.'
[IO.File]::WriteAllText((Join-Path $bundle 'START-HERE.txt'),$notice)
$gameLauncher=@'
$ErrorActionPreference='Stop'
Set-Location $PSScriptRoot
Start-Process -FilePath (Join-Path $PSScriptRoot 'Client/AirsoftClubIntegration.exe') -ArgumentList @('--modern-ui','-screen-fullscreen','0')
'@
[IO.File]::WriteAllText((Join-Path $bundle 'Start-Game.ps1'),$gameLauncher)
$archivePaths=@('Client','Server','compose.yaml','DEVELOPMENT_RUNBOOK.md','MONETIZATION_BALANCE_014.md','VISUAL_FOUNDATION_014.md','VISUAL_BETA_015.md','BUILD-MANIFEST.json','evidence','Start-Backend.ps1','Start-Game.ps1','START-HERE.txt') | ForEach-Object { Join-Path $bundle $_ }
Compress-Archive -Path $archivePaths -DestinationPath (Join-Path $root 'Artifacts/AirsoftClub-Development-Windows-x64.zip') -Force
Get-FileHash (Join-Path $root 'Artifacts/AirsoftClub-Development-Windows-x64.zip') -Algorithm SHA256
