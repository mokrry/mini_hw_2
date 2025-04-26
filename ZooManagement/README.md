# ZooManagement Web API

Решение реализует Web-API для управления зоопарком на базе .NET 8 с использованием принципов Clean Architecture и DDD.

## Структура проектов

- **Domain** — доменные сущности, value-объекты и события.
- **Infrastructure** — интерфейсы репозиториев и их in-memory реализации.
- **Application** — бизнес-логика (сервисы Transfer, Feeding, Statistics).
- **Presentation** — ASP.NET Core Web API (контроллеры, DTO, Swagger).
- **Tests** — модульные тесты (xUnit).

## Требования

- .NET 8 SDK

## Сборка и запуск

1. Клонировать репозиторий и перейти в корень решения:
   ```bash
   git clone <URL> && cd ZooManagement
```markdown
## Сборка и запуск

### Собрать всё решение
```bash
dotnet build
```

### Запустить Web API
```bash
dotnet run --project Presentation/Presentation.csproj
```

### Перейти в браузере
[http://localhost:5000](http://localhost:5000)  
(или порт, указанный в `launchSettings.json`)

### Для просмотра документации Swagger
```bash
http://localhost:5000/swagger
```

---

## HTTP-эндпоинты

### Животные
- `GET    /v1/animals`
- `GET    /v1/animals/{id}`
- `POST   /v1/animals`
- `DELETE /v1/animals/{id}`
- `POST   /v1/animals/{id}/feed`
- `POST   /v1/animals/{id}/treat`
- `POST   /v1/animals/{id}/transfer`

### Вольеры
- `GET    /v1/enclosures`
- `GET    /v1/enclosures/{id}`
- `POST   /v1/enclosures`
- `DELETE /v1/enclosures/{id}`
- `POST   /v1/enclosures/{id}/clean`

### Расписание кормлений
- `GET    /v1/feedschedules`
- `GET    /v1/feedschedules/{id}`
- `POST   /v1/feedschedules`
- `PUT    /v1/feedschedules/{id}`
- `DELETE /v1/feedschedules/{id}`

### Статистика
- `GET    /v1/statistics`

---

## Примеры запросов

```bash
# Создать животное
curl -X POST http://localhost:5000/v1/animals \
  -H "Content-Type: application/json" \
  -d '{
        "species":"Lion",
        "nickname":"Leo",
        "birthDate":"2019-06-01T00:00:00Z",
        "gender":"M",
        "favoriteFood":"Meat"
      }'
```

```bash
# Создать расписание кормления
curl -X POST http://localhost:5000/v1/feedschedules \
  -H "Content-Type: application/json" \
  -d '{
        "animalId":"<GUID>",
        "scheduleTime":"2025-05-01T10:00:00Z",
        "food":"Bananas"
      }'
```

---

## Тестирование

```bash
dotnet test
```