param([ValidateSet('Mono','IL2CPP')][string]$Backend='Mono', [int]$Width=1920, [int]$Height=1080)
$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$root=(Get-Location).Path
$exe=Join-Path $root "Artifacts/$Backend/AirsoftClubIntegration.exe"
$log=Join-Path $root "Artifacts/$Backend-Visual-$Width-$Height.log"
$p=Start-Process -FilePath $exe -WorkingDirectory $root -ArgumentList @('--visual-matrix','-force-d3d11','-screen-width',"$Width",'-screen-height',"$Height",'-screen-fullscreen','0','-logFile',('"'+$log+'"')) -WindowStyle Normal -PassThru
if(!$p.WaitForExit(60000)){Stop-Process -Id $p.Id;throw 'Visual matrix exceeded60seconds'}
if($p.ExitCode -ne 0 -or !(Select-String $log -Pattern 'VISUAL_MATRIX_PASSED' -Quiet)){throw 'Visual matrix failed'}
Select-String $log -Pattern '^VISUAL_MATRIX' | ForEach-Object {$_.Line}
