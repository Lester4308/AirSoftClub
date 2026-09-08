$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$artNames=@('helmet-shell-v2','camouflage-v2','combat-body-v2','arm-front-v2','arm-rear-v2','rifle-v2','smg-v2')
$artRows=foreach($artName in $artNames){
  $artPath=Join-Path $PSScriptRoot "assets/$artName.png"
  $artBitmap=[System.Drawing.Bitmap]::new($artPath)
  try{
    $artClear=0;$artSolid=0;$artSamples=0
    for($artY=0;$artY -lt $artBitmap.Height;$artY+=8){for($artX=0;$artX -lt $artBitmap.Width;$artX+=8){$artAlpha=$artBitmap.GetPixel($artX,$artY).A;$artSamples++;if($artAlpha -eq 0){$artClear++};if($artAlpha -gt 200){$artSolid++}}}
    if($artClear/$artSamples -lt .15 -or $artSolid -lt 100){throw "Invalid transparent sprite: $artName"}
    [pscustomobject]@{name=$artName;width=$artBitmap.Width;height=$artBitmap.Height;transparentSampleFraction=[Math]::Round($artClear/$artSamples,4);opaqueSamples=$artSolid;sha256=(Get-FileHash -LiteralPath $artPath -Algorithm SHA256).Hash}
  }finally{$artBitmap.Dispose()}
}
$artRows|ConvertTo-Json -Depth 4|Set-Content -LiteralPath (Join-Path $PSScriptRoot 'alpha-audit-v2.json') -Encoding utf8
$artRows|Format-Table name,width,height,transparentSampleFraction,opaqueSamples
