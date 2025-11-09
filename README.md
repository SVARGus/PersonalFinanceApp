# PersonalFinanceApp

## Содержание
- [Описание](#описание)
- [Технологический стек](#технологический-стек)
- [Архитектура проекта](#архитектура-проекта)
- [Функциональность](#функциональность)
- [Быстрый старт](#быстрый-старт)
- [Тестирование](#тестирование)
- [Расширение проекта](#расширение-проекта)

## Описание

PersonalFinanceApp - это модульное приложение для учета личных финансов с чистой архитектурой, позволяющее легко заменять пользовательский интерфейс без изменения бизнес-логики.

**Основные возможности:**
- Управление кошельками с поддержкой разных валют
- Учет транзакций (доходы/расходы) с автоматической проверкой баланса
- Финансовая аналитика:
  - Группировка транзакций по типам
  - Топ-3 самых больших трат по кошелькам
  - Баланс в реальном времени
- Гибкие отчеты по месяцам с различными сортировками

Текущая упрощенная реализация через консоль. Структура проекта позволяет заменить UI без изменения логики.

**Будущие варианты UI:**
- Web API (ASP.NET Core)
- WPF Desktop Application
- Blazor Web Assembly
- .NET MAUI (мобильные приложения)

## Технологический стек

### Текущая реализация:
- **C#** (.NET 8.0)
- **xUnit** (unit tests)
- **GitHub** (система контроля версий)
- **Moq** (мокирование для тестов)

### Планируемые технологии:
- **UI**: WPF (десктоп), MAUI (кроссплатформенные приложения), Blazor (веб)
- **Backend**: ASP.NET Core Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity

## Архитектура проекта

```text
PersonalFinanceApp/
├── PersonalFinanceApp.Domain/          # Domain Layer
│   ├── Entities/
│   │   ├── Wallet.cs
│   │   └── Transaction.cs
├── PersonalFinanceApp.Data/            # Data Access Layer
│   ├── Interfaces/
│   │   └── IFinanceRepository.cs
│   └── Repositories/
│       └── InMemoryFinanceRepository.cs
├── PersonalFinanceApp.Services/        # Business Logic Layer
│   └── FinanceService.cs
├── PersonalFinanceApp.ConsoleApp/      # Console UI (Current)
├── PersonalFinanceApp.Tests/           # Unit Tests
├── PersonalFinanceApp.Web/             # Future Web UI
└── PersonalFinanceApp.WPF/             # Future Desktop UI
```

## Функциональность

**Текущая реализация (Console App)**
- Генерация тестовых данных в памяти
- Консольный интерфейс для просмотра отчетов
- Группировка и сортировка транзакций
- Вывод топ-3 трат по кошелькам

## Быстрый старт

### Требования
- .NET 8.0 SDK
- Visual Studio 2022 или VS Code

### Установка и запуск
```
# Клонирование репозитория
git clone <repository-url>
cd PersonalFinanceApp

# Восстановление зависимостей
dotnet restore

# Запуск тестов
dotnet test

# Запуск приложения
dotnet run --project PersonalFinanceApp.ConsoleApp
```

## Тестирование

Проект полностью покрыт модульными тестами с использованием xUnit и Moq:
```
# Запуск всех тестов
dotnet test

# Запуск с детализированным выводом
dotnet test --verbosity normal

# Запуск конкретного тестового проекта
dotnet test PersonalFinanceApp.Tests
```

## Расширение проекта

### Добавление нового UI слоя

### Подключение базы данных

### Добавление новой бизнес-логики

### Принципы разработки
- Clean Architecture - разделение на слои с односторонними зависимостями
- Dependency Injection - легкая замена компонентов
- Repository Pattern - абстракция над источником данных
- Test-Driven Development - высокая надежность кода
- SOLID Principles - соблюдение принципов объектно-ориентированного дизайна

### Дорожная карта
- Базовая архитектура и доменная модель
- Репозиторий с данными в памяти
- Бизнес-логика и сервисный слой
- Консольное приложение
- Модульные тесты
- Подключение базы данных (PostgreSQL + EF Core)
- Web API с ASP.NET Core
- WPF desktop приложение
- Аутентификация и авторизация
- Docker контейнеризация
