FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY common/Common.csproj common/
COPY processing-service/ProcessingService.csproj processing-service/
RUN dotnet restore processing-service/ProcessingService.csproj
COPY common/ common/
COPY processing-service/ processing-service/
WORKDIR /src/processing-service
RUN dotnet publish ProcessingService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ProcessingService.dll"]
