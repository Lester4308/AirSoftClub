$ErrorActionPreference='Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$base='http://127.0.0.1:5080'
$account='dev-http-'+[Guid]::NewGuid().ToString('N').Substring(0,12)
$login=Invoke-RestMethod "$base/dev/login" -Method Post -ContentType 'application/json' -Body (@{Account=$account}|ConvertTo-Json)
$headers=@{Authorization="Bearer $($login.Token)"}
function State { Invoke-RestMethod "$base/api/club" -Headers $headers }
function Command($type,$target='',$value='',$number=0,$flag=$false) {
 $s=State
 $body=@{Key=[Guid]::NewGuid().ToString('N');Version=$s.Version;Type=$type;Target=$target;Value=$value;Number=$number;Flag=$flag;OfferVersion=$s.OfferVersion;CatalogVersion=$s.CatalogVersion}|ConvertTo-Json
 $reply=Invoke-RestMethod "$base/api/command" -Headers $headers -Method Post -ContentType 'application/json' -Body $body
 $retry=Invoke-RestMethod "$base/api/command" -Headers $headers -Method Post -ContentType 'application/json' -Body $body
 if(($reply|ConvertTo-Json -Compress) -ne ($retry|ConvertTo-Json -Compress)){throw 'Retry differs'}
 return $reply
}
$s=State
if($s.Credits -ne 10 -or $s.Fighters.Count -ne 0){throw 'Starter invalid'}
Command 'Hire' $s.Offers[0].Id '' 0 $true | Out-Null
Command 'Buy' 'Pistol-MK1' | Out-Null
$s=State
Command 'Equip' $s.Fighters[0].Id $s.Items[0].Id | Out-Null
Command 'Train' $s.Fighters[0].Id 'Accuracy' | Out-Null
Command 'Heal' $s.Fighters[0].Id | Out-Null
Command 'Refill' '' '' 0 | Out-Null
$rivals=Invoke-RestMethod "$base/api/opponents" -Headers $headers
$beforeBattle=State
$match=Command 'Attack' $rivals.Opponents[0].Id 'Practice'
for($n=0;$n -lt 40;$n++) {
 $result=Invoke-RestMethod "$base/api/match/$($match.MatchId)" -Headers $headers
 if($result.Status -ne 'Pending'){break}
 Start-Sleep -Milliseconds 250
}
if($result.Status -ne 'Completed' -or !$result.Digest){throw 'Battle did not complete'}
$after=State
if($null -eq $result.RewardMoney -or $null -eq $result.RewardClubXp -or $null -eq $result.RewardFighterXp){throw 'Result reward fields absent'}
if(($after.Money-$beforeBattle.Money) -ne $result.RewardMoney -or ($after.Xp-$beforeBattle.Xp) -ne $result.RewardClubXp -or ($after.Fighters[0].Xp-$beforeBattle.Fighters[0].Xp) -ne $result.RewardFighterXp){throw 'Visible reward projection differs from committed state'}
$login2=Invoke-RestMethod "$base/dev/login" -Method Post -ContentType 'application/json' -Body (@{Account=$account}|ConvertTo-Json)
$restored=Invoke-RestMethod "$base/api/club" -Headers @{Authorization="Bearer $($login2.Token)"}
if($restored.Version -ne $after.Version -or $restored.Credits -ne 10 -or $restored.History.Count -ne 1){throw 'Reconnect state invalid'}
@{Account=$account;Match=$match.MatchId;Digest=$result.Digest;Version=$after.Version;Money=$after.Money;Credits=$after.Credits;Status='PASS'}|ConvertTo-Json|Set-Content Artifacts/http-smoke.json
Write-Output "HTTP LOOP PASSED: starter/hire/buy/equip/train/heal/refill/attack/settle/reconnect; duplicate every command verified"
