FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY common/Common.csproj common/
COPY upload-service/UploadService.csproj upload-service/
RUN dotnet restore upload-service/UploadService.csproj
COPY common/ common/
COPY upload-service/ upload-service/
WORKDIR /src/upload-service
RUN dotnet publish UploadService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "UploadService.dll"]
