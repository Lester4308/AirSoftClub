param([switch]$Migrate)
$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$secretPath=Join-Path (Get-Location) 'Artifacts/local-db-password.txt'
if (!(Test-Path $secretPath)) {
 New-Item -ItemType Directory -Force Artifacts | Out-Null
 [IO.File]::WriteAllText($secretPath,[Guid]::NewGuid().ToString('N'))
}
$env:AIRSOFT_DB_PASSWORD=[IO.File]::ReadAllText($secretPath)
$env:AIRSOFT_CONNECTION="Host=127.0.0.1;Port=55432;Database=airsoft_club_dev;Username=airsoft_dev;Password=$env:AIRSOFT_DB_PASSWORD"
docker compose up -d --wait
if($LASTEXITCODE -ne 0){throw 'Development DB startup failed'}
if($Migrate) {
 dotnet run --project src/Airsoft.Server -c Release -- --migrate
 if($LASTEXITCODE -ne 0){throw 'Migration failed'}
}
