# 1) этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Сначала копируем только csproj из общего проекта и из этого сервиса
COPY common/Common.csproj common/Common.csproj
COPY auth-service/AuthService.csproj auth-service/AuthService.csproj

# Делаем restore по проекту сервиса
RUN dotnet restore auth-service/AuthService.csproj

# Теперь копируем весь остальной код (общий + сервис)
COPY common/ common/
COPY auth-service/ auth-service/

# Публикуем
WORKDIR /src/auth-service
RUN dotnet publish AuthService.csproj -c Release -o /app

# 2) этап рантайма
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "AuthService.dll"]
