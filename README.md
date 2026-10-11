# MentorDesk

Платформа Backend School. Первый модуль — карточки для повторения теории в Telegram (вместо Brainscape): колоды, повторение по интервалам, доступ только для активных учеников.

Проект делают ученики школы командой: задачи — в [Issues](https://github.com/backend-school1/mentordesk/issues) и на доске Projects, код — через pull request с ревью.

## Стек

.NET 10 · ASP.NET Core Web API · EF Core + PostgreSQL · Telegram.Bot · xUnit · GitHub Actions

## Структура

```
src/
  MentorDesk.Domain          сущности и правила предметной области, без зависимостей
  MentorDesk.Application     сценарии (сервисы), интерфейсы, DTO
  MentorDesk.Infrastructure  EF Core: AppDbContext, конфигурации, миграции
  MentorDesk.Api             HTTP API, Telegram-бот (Bot/), Program.cs
tests/
  MentorDesk.Tests           xUnit
```

## Запуск за 10 минут

Нужны [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) и Docker.

1. Поднять PostgreSQL:
   ```bash
   docker compose up -d
   ```
2. Создать своего тестового бота: написать [@BotFather](https://t.me/BotFather) → `/newbot` → скопировать токен.
3. Положить токен в user secrets (в репозиторий токен не попадает):
   ```bash
   dotnet user-secrets set "Telegram:BotToken" "<токен>" --project src/MentorDesk.Api
   ```
4. Запустить:
   ```bash
   dotnet run --project src/MentorDesk.Api
   ```
5. Проверить:
   - http://localhost:5190/health — `Healthy`
   - http://localhost:5190/health/ready — `Healthy`, если БД доступна
   - http://localhost:5190/scalar — документация API
   - написать своему боту — он ответит заглушкой

Без токена API тоже запускается, просто без бота.

## Миграции

```bash
dotnet tool restore
dotnet ef migrations add <Имя> --project src/MentorDesk.Infrastructure --startup-project src/MentorDesk.Api
```

При старте приложение само применяет миграции (`Database:MigrateOnStartup`).

## Тесты и стиль

```bash
dotnet test
dotnet format
```

CI проверяет сборку, тесты и форматирование в каждом PR.

## Как участвовать

Правила работы с задачами — в [CONTRIBUTING.md](CONTRIBUTING.md).
