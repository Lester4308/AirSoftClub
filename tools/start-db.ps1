param([switch]$Migrate)
$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$secretPath=Join-Path (Get-Location) '.local/local-db-password.txt'
$legacySecretPath=Join-Path (Get-Location) 'Artifacts/local-db-password.txt'
if (!(Test-Path $secretPath)) {
 # Recover the existing credential before generating anything. PostgreSQL keeps
 # its password in the persistent volume even when build output is deleted.
 $recoveredPassword=$null
 if (Test-Path $legacySecretPath) {
  $recoveredPassword=[IO.File]::ReadAllText($legacySecretPath)
 } else {
  $dbContainers=@(docker ps -a --filter 'label=com.docker.compose.project=airsoft-club-development' --filter 'label=com.docker.compose.service=db' --format '{{.ID}}')
  if($LASTEXITCODE -ne 0){throw 'Cannot inspect Docker; no password was generated.'}
  if($dbContainers.Count -gt 1){throw 'Multiple development DB containers found; no password was generated.'}
  if($dbContainers.Count -eq 1) {
   # Capture inspect output in memory: never print container environment secrets.
   $containerJson=docker inspect $dbContainers[0]
   if($LASTEXITCODE -ne 0){throw 'Cannot inspect development DB container.'}
   $containerInfo=($containerJson -join "`n") | ConvertFrom-Json
   $passwordEntry=@($containerInfo[0].Config.Env | Where-Object { $_.StartsWith('POSTGRES_PASSWORD=') })
   if($passwordEntry.Count -ne 1){throw 'Existing DB credential could not be recovered; preserve its volume.'}
   $recoveredPassword=$passwordEntry[0].Substring('POSTGRES_PASSWORD='.Length)
  } else {
   $existingVolumes=@(docker volume ls --filter 'label=com.docker.compose.project=airsoft-club-development' --format '{{.Name}}')
   if($LASTEXITCODE -ne 0){throw 'Cannot inspect Docker volumes.'}
   if($existingVolumes.Count -gt 0){throw 'Existing DB volume but no recoverable credential. Restore the password; do not delete the volume.'}
   $recoveredPassword=[Guid]::NewGuid().ToString('N')
  }
 }
 if([string]::IsNullOrWhiteSpace($recoveredPassword)){throw 'Empty DB password; no changes made.'}
 New-Item -ItemType Directory -Force (Split-Path $secretPath -Parent) | Out-Null
 [IO.File]::WriteAllText($secretPath,$recoveredPassword)
}
$env:AIRSOFT_DB_PASSWORD=[IO.File]::ReadAllText($secretPath)
$env:AIRSOFT_CONNECTION="Host=127.0.0.1;Port=55470;Database=airsoft_club_dev;Username=airsoft_dev;Password=$env:AIRSOFT_DB_PASSWORD"
docker compose up -d --wait
if($LASTEXITCODE -ne 0){throw 'Development DB startup failed'}
if($Migrate) {
 dotnet run --project src/Airsoft.Server -c Release -- --migrate
 if($LASTEXITCODE -ne 0){throw 'Migration failed'}
}
