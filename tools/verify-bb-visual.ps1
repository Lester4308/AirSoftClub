param([ValidateSet('Mono','IL2CPP')][string]$Backend='Mono')
$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$root=(Get-Location).Path
$log=Join-Path $root "Artifacts/$Backend-BbVisual.log"
$p=Start-Process -FilePath (Join-Path $root "Artifacts/$Backend/AirsoftClubIntegration.exe") -ArgumentList @('--bb-visual','-force-d3d11','-screen-width','1920','-screen-height','1080','-screen-fullscreen','0','-logFile',('"'+$log+'"')) -WindowStyle Normal -PassThru
if(!$p.WaitForExit(60000)){Stop-Process -Id $p.Id;throw 'BB visual exceeded60seconds'}
if($p.ExitCode -ne 0 -or !(Select-String $log -Pattern 'BB_VISUAL_PASSED' -Quiet)){throw 'BB visual failed'}
Select-String $log -Pattern '^BB_VISUAL' | ForEach-Object {$_.Line}

Add-Type -AssemblyName System.Drawing
Add-Type -ReferencedAssemblies System.Drawing -TypeDefinition @'
using System;
using System.Drawing;
public static class BbPixelProof015 {
 public static int Count(string file,int red,int green,int blue) {
  using(var b=new Bitmap(file)){int count=0;for(int y=380;y<945;y++)for(int x=300;x<1860;x++){
   var c=b.GetPixel(x,y);if(Math.Abs(c.R-red)<=2&&Math.Abs(c.G-green)<=2&&Math.Abs(c.B-blue)<=2)count++;
  }return count;}
 }
}
'@
$colors=@(@(239,244,234),@(115,215,128),@(110,179,255),@(237,117,106),@(201,141,250))
$proof=@()
for($tier=0;$tier -lt 5;$tier++){
 $c=$colors[$tier]
 $count=[BbPixelProof015]::Count((Join-Path $root "Artifacts/club-bb-tier-$tier.png"),$c[0],$c[1],$c[2])
 if($count -lt 4){throw "Tier$tier lacks projectile-colored pixels inside battlefield"}
 $proof+=@{Tier=$tier;FieldPixels=$count;Rgb=($c -join ',')}
}
$proof|ConvertTo-Json|Set-Content (Join-Path $root "Artifacts/$Backend-BbPixels.json")
Write-Output 'BB PIXEL PROOF PASSED: all5 colors present in battlefield crop, excluding HUD/legend'
