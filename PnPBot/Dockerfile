FROM mcr.microsoft.com/dotnet/sdk:6.0-bookworm-slim-arm64v8 AS build-env
WORKDIR /App
COPY .. .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0-bookworm-slim-arm64v8
RUN apt-get update && apt-get install -y openjdk-17-jre-headless
WORKDIR /DiscordBot

COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "DiscordBot/PnPBot.dll"]
