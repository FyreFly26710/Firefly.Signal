# Local Docker And Compose

This document defines the backend container strategy for local development.

## Goals
- make local infrastructure easy to boot
- match the eventual Docker-based deployment model
- avoid framework-specific orchestration
- keep normal backend iteration fast

## Required Local Infrastructure

Use Docker Compose for:
- PostgreSQL
- pgweb
- Redis
- RedisInsight
- RabbitMQ

Recommended compose file location:
- `services/api/docker-compose.yml`

## Recommended Service Names
- `postgres`
- `pgweb`
- `redis`
- `redisinsight`
- `rabbitmq`

These names should be used consistently in local connection strings.

## Recommended Ports
- PostgreSQL: `5432`
- pgweb: `5050`
- Redis: `6379`
- RedisInsight: `5540`
- RabbitMQ: `5672`
- RabbitMQ Management: `15672`

## Recommended Compose Template

```yaml
services:
  postgres:
    image: postgres:17
    environment:
      POSTGRES_USER: firefly
      POSTGRES_PASSWORD: firefly
      POSTGRES_DB: firefly_signal
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data

  pgweb:
    image: sosedoff/pgweb:0.16.2
    environment:
      DATABASE_URL: postgres://firefly:firefly@postgres:5432/firefly_signal?sslmode=disable
    ports:
      - "5050:8081"
    depends_on:
      - postgres

  redis:
    image: redis:8
    ports:
      - "6379:6379"

  redisinsight:
    image: redis/redisinsight:2.70
    ports:
      - "5540:5540"
    depends_on:
      - redis

  rabbitmq:
    image: rabbitmq:4-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest

volumes:
  postgres-data:
```

## Connection String Conventions

Inside API containers:
- PostgreSQL: `Host=postgres;Port=5432;Database=...;Username=firefly;Password=firefly`
- Redis: `redis:6379`
- RabbitMQ:
  - `RabbitMq__Host=rabbitmq`
  - `RabbitMq__Port=5672`
  - `RabbitMq__Username=guest`
  - `RabbitMq__Password=guest`
  - `EventBus__ExchangeName=firefly_signal_event_bus` when overriding defaults

When running APIs locally outside Docker:
- use `localhost` hostnames

## API Dockerfiles

Each API should own its own Dockerfile, located beside its project file.

Examples:
- `services/api/src/Firefly.Signal.Ai.Api/Dockerfile`
- `services/api/src/Firefly.Signal.Gateway.Api/Dockerfile`
- `services/api/src/Firefly.Signal.Identity.Api/Dockerfile`
- `services/api/src/Firefly.Signal.JobSearch.Api/Dockerfile`

Recommended Dockerfile template:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore services/api/src/Firefly.Signal.JobSearch.Api/Firefly.Signal.JobSearch.Api.csproj
RUN dotnet publish services/api/src/Firefly.Signal.JobSearch.Api/Firefly.Signal.JobSearch.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Firefly.Signal.JobSearch.Api.dll"]
```

Use exact SDK/runtime tags that match the repo's chosen .NET 10 build policy when implementation starts.

## Local Development Practice

Recommended day-to-day flow:
1. start infrastructure via Compose
2. run APIs with `dotnet run`
3. use Dockerfiles when testing containerized behavior or deployment parity

This keeps normal coding fast without losing Docker alignment.

## Recommended Rule Set
- Compose owns shared local infrastructure.
- Dockerfiles own per-API runtime packaging.
- APIs should not require special orchestration frameworks to run locally.
- Keep frontend containers out of the first backend compose file.
