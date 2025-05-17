FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY common/Common.csproj common/
COPY transcription-service/TranscriptionService.csproj transcription-service/
RUN dotnet restore transcription-service/TranscriptionService.csproj
COPY common/ common/
COPY transcription-service/ transcription-service/
WORKDIR /src/transcription-service
RUN dotnet publish TranscriptionService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "TranscriptionService.dll"]
