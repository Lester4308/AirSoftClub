# Research basis і provenance

## Поточний статус — v3

Активні product rules: [Gate v3](AIRSOFT_CLUB_GAME_PRODUCT_DESIGN_GATE_v3.md), [реєстр V3-00–V3-76](PRODUCT_DECISIONS.md). [User decisions v3](USER_APPROVED_DECISIONS_v3.txt) і [friend clarification](USER_CLARIFICATION_v3.md) замінюють суперечливі v1/v2 product assumptions.

Розділи нижче — **історичний звіт дослідження від 2026-09-05**, а не активна специфікація. Згадки original 3 slots, offer counts, Energy або unknown original formulas зберігають provenance і не відновлюють superseded rules нової гри. Source manifest не змінено. У цьому v3 pass архів не перечитувався і не редагувався; нове historical confirmation не заявляється.

Сучасні official references для поточного stack/payment design наведені у [Backend](BACKEND_ARCHITECTURE.md) та [Steam architecture](STEAM_SOCIAL_PVP_ARCHITECTURE.md). Перевірка документації не є реалізацією інтеграцій.

## Обсяг перевірки

Повністю прочитані 7 названих користувачем основних документів, усі тематичні fighter/team/UI/social/combat/economy документи та item/weapon/economy CSV, додаткові version/history/API/asset/loading notes. Окремо візуально перевірено 3 ключові кадри: fighter profile,opponent list,result. Повного повторного перегляду двох відео по кадрах у цьому завданні не проводилося; використано готовий time-coded VIDEO_ANALYSIS і ledger. Сирі HTTP архіви, бінарники і всі 649 файлів не називаються повторно перевіреними.

## Основні локальні джерела

| Reference | Джерело | Для чого |
|---|---|---|
| R01 | [Master research](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/00_MASTER/PAINTBALL_WARS_MASTER_RESEARCH.md>) | Загальний контекст та unknowns |
| R02 | [Evidence ledger](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/00_MASTER/EVIDENCE_LEDGER.csv>) | Claim-level E001–E034 і confidence |
| R03 | [Video analysis](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/01_VIDEO/VIDEO_ANALYSIS.md>) | Часові прив'язки двох gameplay videos |
| R04 | [Screenshot catalog](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/02_SCREENSHOTS/SCREENSHOT_CATALOG.md>) | Зміст збережених кадрів |
| R05 | [Combat reconstruction](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/08_GAME_SYSTEMS/COMBAT_SYSTEM_RECONSTRUCTION.md>) | Fixed positions, feedback, unknown formulas |
| R06 | [Economy reconstruction](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/09_ECONOMY/ECONOMY_RECONSTRUCTION.md>) | Ресурси, observed values, ambiguity |
| R07 | [Original reconstruction spec](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/12_RECONSTRUCTION/PAINTBALL_WARS_RECONSTRUCTION_SPEC_v1.md>) | Відокремлення original від reconstruction decisions |
| R08 | [Fighters](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/08_GAME_SYSTEMS/FIGHTER_SYSTEM.md>) | Stats і профілі |
| R09 | [Roster](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/08_GAME_SYSTEMS/TEAM_AND_ROSTER.md>) |16cap, active/reserve unknown |
| R10 | [Social](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/08_GAME_SYSTEMS/SOCIAL_AND_PVP.md>) | Асинхронність strongly supported, рейтинг/revenge unknown |
| R11 | [UI](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/08_GAME_SYSTEMS/UI_SCREEN_MAP.md>) | Flow topology |
| R12 | [Version differences](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/10_HISTORY/VERSION_DIFFERENCES.md>) | Не змішувати порти/версії |

## Ключові evidence anchors

| IDs | Спостереження | Timestamp / межа висновку |
|---|---|---|
| E002 | Офіційний опис 3stats,16cap,team customization | Не доводить 16 одночасно deployed |
| E007 | Free recruit,8candidates | Video1 00:35 |
| E008 | Weapon price/stat/resale | Video1 01:10; не new-game balance |
| E009 | Friend Attack/Profile | Video1 03:10 |
| E010/E011 |3slots,stats,training1,set reference | Video1 01:50/02:00; set formula UNKNOWN |
| E012/E020 | Auto battle/skip/fixed positions/MISS | Video1 02:10–02:30; Video2 06:40–11:40 |
| E014 | Opponent/allies lists і 16fighter card | Video1 04:50; matching formula UNKNOWN |
| E015 |100ammo/50coins,200/100 | Video1 05:20;2013version |
| E016/E017 |44/44,36/32,97/84 rewards | Video1 05:40; Video2 05:00; не формула |
| E018 | L3train2,10/10/12stats | Video2 02:10; cost dependence UNKNOWN |
| E021 | Missing mask validation | Video1 09:30 |
| E024/E025/E026 | Energy conflict | Guides проти окремого вислову в traffic note |
| E026/E027 | Server route/replay descriptions | Passive archive evidence; no live execution |
| E029 | Early500/9/1000,XP0/40 | Version-specific UI baseline |

## Візуально оглянуті файли

- [Fighter profile](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/02_SCREENSHOTS/video1_0110_fighter_profile.jpg>): три slots,profile stats,training cost; stylized portrait. Не підстава переносити old art.
- [Club opponents](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/02_SCREENSHOTS/video1_0290_club.jpg>): cards/Attack,opponents/allies,16 на картці союзника.
- [Battle result](<C:/Users/Ihor/.codex/.chatgpt-projects/g-p-6a9bed7035f48191b412ad56f7ccb7b9/PaintballWars_Research/02_SCREENSHOTS/video1_0340_battle_result.jpg>):44XP/44coins,skip,team aggregate bars.

## Confidence corrections для нового продукту

Master executive summary називає гру async категорично; тематичний Social note обґрунтовано залишає STRONGLY SUPPORTED. Цей pack користується обережнішою оцінкою. Snapshot architecture не доведена історично. Reconstruction spec рекомендує 16slots,500coins,karma10 — це compatibility profile іншої задачі, не автоматичні вимоги Airsoft.

HP видно; healing timing/cost не встановлено. «Damage1–2» не дорівнює final4–7; coefficients невідомі. Сильні weapon/build tradeoffs нової гри — design objective, а не емпірично доведена властивість оригіналу. Leaderboards/revenge/paid monetization не реконструюються з порожнечі.

## Сучасні технічні джерела

Steam claims локалізовано разом із посиланнями у [Steam architecture](STEAM_SOCIAL_PVP_ARCHITECTURE.md): офіційні Auth,ISteamUserAuth,ISteamFriends,ISteamUser,Leaderboards,Game Notifications. Перевірка — читання документації, не запуск production integration. Немає припущення, що SDK friend enumeration дорівнює server authorization або що optional revenge є Steam turn notification.
