FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY common/Common.csproj common/
COPY search-service/SearchService.csproj search-service/
RUN dotnet restore search-service/SearchService.csproj
COPY common/ common/
COPY search-service/ search-service/
WORKDIR /src/search-service
RUN dotnet publish SearchService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "SearchService.dll"]
