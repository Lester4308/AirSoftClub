$ErrorActionPreference='Stop'
. "$PSScriptRoot/start-db.ps1" -Migrate
Add-Type -AssemblyName System.Net.Http
$oldAsp=$env:ASPNETCORE_ENVIRONMENT
$oldDotnet=$env:DOTNET_ENVIRONMENT
$oldAuth=$env:AIRSOFT_DEV_AUTH
$env:ASPNETCORE_ENVIRONMENT='Production'
$env:DOTNET_ENVIRONMENT='Production'
$env:AIRSOFT_DEV_AUTH='1' # Production must still reject development routes.
$server=$null
$http=[System.Net.Http.HttpClient]::new()
try {
 $server=Start-Process dotnet -ArgumentList @('Artifacts/bin/Airsoft.Server/Release/net10.0/Airsoft.Server.dll','--urls','http://127.0.0.1:5082') -WindowStyle Hidden -PassThru -RedirectStandardOutput Artifacts/security-server.log -RedirectStandardError Artifacts/security-server-error.log
 $ready=$false
 for($n=0;$n -lt 30;$n++) {
  try {$r=$http.GetAsync('http://127.0.0.1:5082/ready').GetAwaiter().GetResult();$ready=$r.IsSuccessStatusCode;$r.Dispose()} catch {}
  if($ready){break}; Start-Sleep -Milliseconds 250
 }
 if(!$ready){throw 'Production security fixture failed to start'}
 $body=[System.Net.Http.StringContent]::new('{"Account":"dev-security"}',[Text.Encoding]::UTF8,'application/json')
 $r=$http.PostAsync('http://127.0.0.1:5082/dev/login',$body).GetAwaiter().GetResult()
 if([int]$r.StatusCode -ne 401){throw 'Production exposed dev login'}
 $r.Dispose();$body.Dispose()
 foreach($path in @('/api/club','/api/dev/ledger','/api/dev/command')) {
  $r=$http.GetAsync('http://127.0.0.1:5082'+$path).GetAwaiter().GetResult()
  if([int]$r.StatusCode -ne 401){throw "Unauthenticated route exposed: $path"};$r.Dispose()
 }
 $limited=$false
 for($n=0;$n -lt 245;$n++) {
  $r=$http.GetAsync('http://127.0.0.1:5082/health').GetAwaiter().GetResult()
  if([int]$r.StatusCode -eq 429){$limited=$true};$r.Dispose();if($limited){break}
 }
 if(!$limited){throw 'Rate limit did not reject excess requests'}
 Write-Output 'SECURITY PASS: Production rejects dev login even with flag=1; protected routes require auth; rate limit returns429'
} finally {
 $http.Dispose()
 if($server -and !$server.HasExited){Stop-Process -Id $server.Id}
 $env:ASPNETCORE_ENVIRONMENT=$oldAsp
 $env:DOTNET_ENVIRONMENT=$oldDotnet
 $env:AIRSOFT_DEV_AUTH=$oldAuth
}
