$ErrorActionPreference='Stop'
. "$PSScriptRoot/start-db.ps1" -Migrate
$env:ASPNETCORE_ENVIRONMENT='Development'
$env:AIRSOFT_DEV_AUTH='1'
$env:ASPNETCORE_URLS='http://127.0.0.1:5080'
dotnet run --project src/Airsoft.Server -c Release --no-launch-profile
if($LASTEXITCODE -ne 0){throw 'Server stopped with error'}
