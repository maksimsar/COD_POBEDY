FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY common/Common.csproj common/
COPY metadata-service/MetadataService.csproj metadata-service/
RUN dotnet restore metadata-service/MetadataService.csproj
COPY common/ common/
COPY metadata-service/ metadata-service/
WORKDIR /src/metadata-service
RUN dotnet publish MetadataService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "MetadataService.dll"]
