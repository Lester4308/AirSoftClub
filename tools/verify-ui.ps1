param([ValidateSet('Mono','IL2CPP')][string]$Backend='IL2CPP', [switch]$Visible)
$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$root=(Get-Location).Path
$exe=Join-Path $root "Artifacts/$Backend/AirsoftClubIntegration.exe"
$log=Join-Path $root "Artifacts/$Backend-UiSmoke.log"
$windowStyle=if($Visible){'Normal'}else{'Hidden'}
$p=Start-Process -FilePath $exe -WorkingDirectory $root -ArgumentList @('--club-ui-smoke','-force-d3d11','-screen-width','1280','-screen-height','800','-screen-fullscreen','0','-logFile',('"'+$log+'"')) -WindowStyle $windowStyle -PassThru
if(!$p.WaitForExit(60000)){Stop-Process -Id $p.Id;throw 'UI smoke exceeded60seconds'}
if($p.ExitCode -ne 0 -or !(Select-String $log -Pattern 'CLUB_UI_SMOKE_PASSED' -Quiet)){throw 'UI smoke failed'}
$reconnectLog=Join-Path $root "Artifacts/$Backend-UiReconnect.log"
$reconnect=Start-Process -FilePath $exe -WorkingDirectory $root -ArgumentList @('--club-reconnect-smoke','-force-d3d11','-screen-width','1280','-screen-height','800','-screen-fullscreen','0','-logFile',('"'+$reconnectLog+'"')) -WindowStyle $windowStyle -PassThru
if(!$reconnect.WaitForExit(60000)){Stop-Process -Id $reconnect.Id;throw 'Reconnect smoke exceeded60seconds'}
if($reconnect.ExitCode -ne 0 -or !(Select-String $reconnectLog -Pattern 'CLUB_RECONNECT_PASSED' -Quiet)){throw 'Separate process reconnect failed'}
Write-Output 'UI backend loop passed. Inspect screenshots separately; black frames do not pass visual QA.'
