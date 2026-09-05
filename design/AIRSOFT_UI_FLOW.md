# Airsoft UI flow

NEW AIRSOFT DESIGN; reference E006–E023. Production UI art не створюється на цьому етапі.

## Navigation

Launch → Auth → Club; із Club постійні tabs Team / Training / Shop / Opponents / History. Leaderboards доступний з Club і Opponents. Settings завжди доступний поза транзакційним modal. Desktop navigation не залежить від illustrated city.

| Screen | Основний зміст | CTA / перехід | Empty/error behavior |
|---|---|---|---|
| Auth | Steam status, backend status | retry / offline practice | auth failure не створює фальшивий online profile |
| Club | own logo/name,level,rating,Credits,ready trio,defense summary | Prepare / Opponents | cache badge і час останньої синхронізації |
| Team | deployed3, reserve до 3, preset | Fighter Detail / Recruit / Save squad | зміна preset атомарна; cancel не змінює defense |
| Fighter Detail | три stats, XP budget,3slots | compare/equip/train/rename | конфлікт revision → reload + зберегти вибір у UI |
| Recruit |4 рівнобюджетні candidates,price | hire | max roster пояснюється, не відкривається purchase |
| Training Center | доступний upgrade budget,ціна,preview | apply/respec | недостатньо XP/Credits — показати причину |
| Shop | Primary/Protection/Kit,filters,side-by-side | buy / buy+equip explicit | вже owned/locked/no funds окремо |
| Opponents | Steam Friends / Rating Match / Revenge | Preview | no friends ≠ SDK error; rating/PvE лишається |
| Club Profile | Steam identity + окремий Club; public lineup,rating,records | challenge/report/block | inaccessible club без витоку приватних полів |
| Preview |3v3,map,squad,defense snapshot time,cost/reward/rated badge | Accept match | expired offer/snapshot → refresh preview, без silent substitution |
| Battle |6participants,round score,stamina,hit/out feedback | pause playback/speed/skip | disconnect не скасовує серверний match |
| Result | already settled reward,rating delta,XP,outcome,3key facts | next opponent / prepare / replay | pending settlement → pending screen, без вигаданої виплати |
| History | Attacks / Defenses,datetime,mode,opponent | replay / eligible revenge | expired revenge лишає replay |
| Leaderboards | Global / Friends / Around Me | view club/challenge | friends unavailable → явне повідомлення, не порожній global |

## Opponent cards

Steam avatar і display name; Club name окремим рядком; rating; strength estimate із tooltip «орієнтир, не прогноз перемоги»; default format3v3. Recent form — останні 5 відповідного mode; не змішувати ranked і casual wins. Last active прихований v1. Placeholder avatar/unknown name не блокують матч.

Challenge з leaderboard — unranked; для rating використовувати Rating Match. Так leaderboard не стає обходом ranked offers.

## Defense report

«Ваш клуб атакував [Steam name] / [Club name]». Perspective чіткий: «Ваш захист: перемога/поразка/нічия». Показати rating delta, що вже applied; resources change0 на defense; timestamp і mode. CTA Revenge, якщо ticket valid, і Replay. Reports агрегуються; користувач може вимкнути rivalry alerts.

## Network state machine

Draft → validating → accepted(matchId) → resolving → settled → playback/result. Повтор submit використовує той самий idempotency key. При timeout після submit клієнт шукає command/match status, не створює новий battle. При reconnect показує settlement, який відбувся offline. Training/equip mutations із stale revision відхиляються з reload; немає last-write-wins для wallet.

## Accessibility і moderation surfaces

Клавіатурний focus, видимий selected state, text scale, reduced motion, HIT/OUT текстом. Усі hover дані доступні натисканням. User text plain-text escaped, довжина name обмежена. Club names проходять normalization, filter, report і moderator rename; невинні збіги мають appeal. Власні fighter names не публікуються без потреби; базова report/block дія присутня. Emblem v1 тільки curated layers.
