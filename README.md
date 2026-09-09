# Airsoft_Club_Game

Modern UGUI та нові Volumetric017-спрайти активні. **Гра ще має відкриті UI-дефекти:** вікно екіпірування блокує CLOSE/EQUIP, Settings відкриває Club; частина функцій старого UI не перенесена. Успішна компіляція не означає повну працездатність. Див. [діагностику](implementation/DIAGNOSTIC_AUDIT_2026-09-09.md).

## Запуск у Windows PowerShell

Відкрий Docker Desktop. У PowerShell:

```powershell
Set-Location 'C:\Users\Ihor\Documents\ChatGPT\Airsoft_Club_Game'
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\run-development.ps1
```

Скрипт запускає PostgreSQL, застосовує міграції, налаштовує development-авторизацію та запускає сервер на http://127.0.0.1:5080. Залиш це вікно відкритим. Якщо сервер уже працює, не запускай другу копію на тому самому порті.

В іншому вікні PowerShell:

```powershell
Set-Location 'C:\Users\Ihor\Documents\ChatGPT\Airsoft_Club_Game'
# Якщо збірки немає або код змінився:
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\verify-unity.ps1 -Stage MonoBuild
Start-Process .\Artifacts\Mono\AirsoftClubIntegration.exe
```

Unity: відкривати `UnityHost` редактором 6000.3.21f1. Modern UI вмикається за замовчуванням.

## Локальні дані

Чинний пароль development-БД зберігається в `.local/local-db-password.txt`. Каталог виключено з Git; він не є кешем і не підлягає очищенню. Скрипт підтримує старий шлях Artifacts/local-db-password.txt як джерело міграції та відновлення пароля з наявного development-контейнера. Якщо існує volume без відновлюваного пароля, скрипт зупиниться замість створення несумісного пароля. Не видаляй volume для обходу цієї помилки.

## Перевірки

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\verify.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\verify-server.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\verify-unity.ps1 -Stage EditMode
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\verify-unity.ps1 -Stage PlayMode
```

Останній PlayMode після очищення: **7 PASS / 1 FAIL**. DiagnosticAuditTests відтворює відомі дефекти інвентарю та Settings. Не видаляти або вимикати тест, щоб штучно отримати зелений результат. GoldenRuntimeSmoke проходить: ресурси golden-* знаходяться в локальному пакеті `src/Airsoft.Battle/Runtime/Resources`, а не лише в UnityHost/Assets.

## Структура

- `UnityHost/` — Unity-клієнт, сцени, ресурси та UI-тести.
- `src/`, `tests/` — ядро бою, клуб, сервер і .NET-тести.
- `art/` — джерела та походження арту.
- `design/`, `implementation/` — рішення, звіти та докази перевірок.
- `tools/` — запуск, збірка й перевірки.
- `Artifacts/` — результати збірок; перед очищенням перевіряти локальні службові файли.
- `.local/` — приватні локальні налаштування, не видаляти.

Стек: Unity/C#, ASP.NET Core/.NET 10, PostgreSQL **18.6** згідно з compose.yaml. Native IL2CPP/Steam sandbox/final art потребують окремої перевірки; історичні результати не підтверджують поточну збірку.

## Документація

- [Правила проєкту](AGENTS.md)
- [Межа незалежності](PROJECT_BOUNDARY.md)
- [Master spec](AIRSOFT_CLUB_GAME_MASTER_DEVELOPMENT_SPEC_v1.md)
- [Повна діагностика](implementation/DIAGNOSTIC_AUDIT_2026-09-09.md)
- [Виправлений звіт очищення](CLEANUP_REPORT.md)
- [Журнал відновлення після очищення](implementation/CLEANUP_RECOVERY_2026-09-09.md)
