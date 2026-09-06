param(
 [ValidateSet('EditMode','PlayMode','MonoBuild','IL2CPPBuild','MonoRun','IL2CPPRun')]
 [string]$Stage = 'EditMode',
 [string]$Editor = 'C:\Program Files\Unity\Hub\Editor\6000.3.21f1\Editor\Unity.exe'
)
$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$root = (Get-Location).Path
$log = Join-Path $root ("Artifacts/" + $Stage + ".log")
if ($Stage.EndsWith('Run')) {
 $folder = $Stage.Replace('Run','')
 $exe = Join-Path $root ("Artifacts/" + $folder + "/AirsoftClubIntegration.exe")
 $arguments = @('-batchmode','-nographics','--airsoft-smoke','-logFile',('"' + $log + '"'))
} else {
 $exe = $Editor
 $arguments = @('-batchmode','-nographics','-projectPath',('"' + (Join-Path $root 'UnityHost') + '"'),'-logFile',('"' + $log + '"'))
 if ($Stage.EndsWith('Build')) {
  $method = if($Stage -eq 'MonoBuild') { 'BuildGate.Managed' } else { 'BuildGate.Il2Cpp' }
  $arguments += @('-quit','-executeMethod',$method)
 } else {
  $results = Join-Path $root ("Artifacts/" + $Stage + ".xml")
  $arguments += @('-runTests','-testPlatform',$Stage,'-testResults',('"' + $results + '"'))
 }
}
$process = Start-Process -FilePath $exe -ArgumentList $arguments -WindowStyle Hidden -PassThru
$process.WaitForExit()
if ($process.ExitCode -ne 0) { throw "$Stage exit $($process.ExitCode); see $log" }
if ($Stage -eq 'EditMode' -or $Stage -eq 'PlayMode') {
 [xml]$xml = Get-Content -LiteralPath $results
 $run = $xml.'test-run'
 if ([int]$run.failed -ne 0 -or [int]$run.passed -lt 1) { throw "$Stage test failures/empty run" }
 Write-Output "$Stage passed=$($run.passed) failed=$($run.failed) skipped=$($run.skipped)"
} else {
 $marker = if ($Stage.EndsWith('Run')) { 'AIRSOFT_PLAYER_PASSED' } else { 'AIRSOFT_BUILD_PASSED' }
 if (-not (Select-String -LiteralPath $log -Pattern $marker -Quiet)) { throw "Missing $marker" }
}
Select-String -LiteralPath $log -Pattern '^PERF |^AIRSOFT_' | ForEach-Object { $_.Line }
