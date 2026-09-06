$ErrorActionPreference='Stop'
. "$PSScriptRoot/start-db.ps1" -Migrate
dotnet run --project tests/Airsoft.Server.Tests -c Release
if($LASTEXITCODE -ne 0){throw 'Actual PostgreSQL tests failed'}
