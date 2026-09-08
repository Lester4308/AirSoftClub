Add-Type -AssemblyName System.Drawing
$root=$PSScriptRoot
$im=[System.Drawing.Bitmap]::new((Join-Path $root 'atlas.png'))
$regions=@(@('body-olive',40,0,330,455),@('body-camo',410,0,330,455),@('head-bare',760,0,370,430),@('head-equipped',1150,0,386,430),@('arm-front-olive',30,455,345,260),@('arm-front-camo',420,455,330,260),@('arm-rear-olive',770,455,345,260),@('arm-rear-camo',1150,455,386,260),@('vest',30,720,330,304),@('rifle',370,760,400,240))
$items=@()
foreach($r in $regions){
 $left=1536;$top=1024;$right=0;$bottom=0
 for($y=$r[2];$y -lt ($r[2]+$r[4]);$y++){for($x=$r[1];$x -lt ($r[1]+$r[3]);$x++){if($im.GetPixel($x,$y).A -gt 0){$left=[Math]::Min($left,$x);$top=[Math]::Min($top,$y);$right=[Math]::Max($right,$x);$bottom=[Math]::Max($bottom,$y)}}}
 $rect=[System.Drawing.Rectangle]::new($left,$top,$right-$left+1,$bottom-$top+1)
 $crop=$im.Clone($rect,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
 $crop.Save((Join-Path $root ('sprites/'+$r[0]+'.png')),[System.Drawing.Imaging.ImageFormat]::Png)
 $crop.Dispose()
 $items+=@{name=$r[0];file=('sprites/'+$r[0]+'.png');rect=@($left,$top,$rect.Width,$rect.Height)}
}
$im.Dispose()
$items | ConvertTo-Json -Depth 5 | Set-Content (Join-Path $root 'atlas-rects.json') -Encoding utf8
$items | ConvertTo-Json -Compress
