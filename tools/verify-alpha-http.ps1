$ErrorActionPreference='Stop'
. "$PSScriptRoot/verify-http.ps1"
function Dev($type,$target='') {
 $s=State
 $body=@{Key=[Guid]::NewGuid().ToString('N');Version=$s.Version;Type=$type;Target=$target}|ConvertTo-Json
 $first=Invoke-RestMethod "$base/api/dev/command" -Headers $headers -Method Post -ContentType 'application/json' -Body $body
 $second=Invoke-RestMethod "$base/api/dev/command" -Headers $headers -Method Post -ContentType 'application/json' -Body $body
 if(($first|ConvertTo-Json -Compress) -ne ($second|ConvertTo-Json -Compress)){throw 'Development retry mismatch'}
}
function ExpectRejected($body) {
 try { Invoke-RestMethod "$base/api/command" -Headers $headers -Method Post -ContentType 'application/json' -Body ($body|ConvertTo-Json) | Out-Null; throw 'Expected rejection' }
 catch { if([int]$_.Exception.Response.StatusCode -notin @(400,409)){throw} }
}
Dev 'Grant'
Command 'Name' '' 'Alpha Test Club' | Out-Null
Command 'Emblem' '' '' 3 | Out-Null
Command 'Daily' | Out-Null
Command 'Progression' | Out-Null
$s=State
if($s.Streak -ne 1 -or $s.Emblem -ne 3 -or $s.Name -ne 'Alpha Test Club' -or !$s.DefensePublished){throw 'Missing profile projection'}
Command 'Train' $s.Fighters[0].Id 'Endurance' | Out-Null
Command 'Heal' $s.Fighters[0].Id '' 10 | Out-Null
Dev 'Recovery'
Dev 'Refresh'
Command 'Refresh' '' '' 0 $true | Out-Null
$s=State
ExpectRejected @{Key='too-early';Version=$s.Version;Type='Refresh';CatalogVersion=$s.CatalogVersion}
ExpectRejected @{Key='level1-early';Version=$s.Version;Type='EarlyUnlock';Target='HeadProtection-3';CatalogVersion=$s.CatalogVersion}
Command 'Buy' 'HeadProtection-1' | Out-Null
$s=State
$armor=$s.Items|Where-Object Definition -eq 'HeadProtection-1'|Select-Object -First 1
Command 'Equip' $s.Fighters[0].Id $armor.Id | Out-Null
Command 'Refill' '' '' 4 | Out-Null
Command 'BbTier' '' '' 4 | Out-Null
Command 'Shield' '' '' 8 | Out-Null
$s=State
if($s.ShieldUntil -le $s.ServerNow -or $s.BbStock[4] -ne 500 -or $s.ActiveBbTier -ne 4){throw 'Resource/shield persistence failed'}
$target=$rivals.Opponents[0].Id
Dev 'Revenge' $target
$s=State
$ticket=$s.RevengeTickets|Where-Object Target -eq $target|Select-Object -Last 1
$payload=@{Key=[Guid]::NewGuid().ToString('N');Version=$s.Version;Type='Attack';Target=$target;Value='Revenge';Ticket=$ticket.Origin}|ConvertTo-Json
$revenge=Invoke-RestMethod "$base/api/command" -Headers $headers -Method Post -ContentType 'application/json' -Body $payload
$again=Invoke-RestMethod "$base/api/command" -Headers $headers -Method Post -ContentType 'application/json' -Body $payload
if($revenge.MatchId -ne $again.MatchId){throw 'Duplicate revenge match'}
for($n=0;$n -lt 40;$n++) {
 $result=Invoke-RestMethod "$base/api/match/$($revenge.MatchId)" -Headers $headers
 if($result.Status -eq 'Completed'){break}
 Start-Sleep -Milliseconds 250
}
$s=State
if($result.Status -ne 'Completed' -or !$result.RatingKnown -or $s.ShieldUntil -ne 0){throw 'Revenge settlement/rated shield cancellation failed'}
$ticketAfter=$s.RevengeTickets|Where-Object Origin -eq $ticket.Origin
if($ticketAfter -and $ticketAfter.Attempts -ne 1){throw 'Revenge attempts duplicated'}
$ledger=Invoke-RestMethod "$base/api/dev/ledger" -Headers $headers
if($ledger.Entries.Count -lt 10){throw 'Ledger inspection missing'}
$saved=$s
$login=Invoke-RestMethod "$base/dev/login" -Method Post -ContentType 'application/json' -Body (@{Account=$account}|ConvertTo-Json)
$headers=@{Authorization="Bearer $($login.Token)"}
$restored=State
if($restored.Version -ne $saved.Version -or $restored.Money -ne $saved.Money -or $restored.Credits -ne $saved.Credits -or $restored.OfferVersion -ne $saved.OfferVersion -or $restored.Emblem -ne 3 -or $restored.Streak -ne 1 -or $restored.History.Count -ne 2){throw 'Alpha reconnect differs'}
@{Status='PASS';Account=$account;Version=$restored.Version;History=$restored.History.Count;LedgerEntries=$ledger.Entries.Count;Revenge=$revenge.MatchId;Contract=$restored.ContractVersion}|ConvertTo-Json|Set-Content Artifacts/alpha-http.json
Write-Output 'ALPHA HTTP PASSED: profile/daily/progression/training/partial-heal/recovery/refresh/early-unlock/armor/BB/shield/Revenge/ledger/reconnect'
