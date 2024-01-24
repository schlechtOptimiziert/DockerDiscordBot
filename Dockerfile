FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
RUN apt-get update && apt-get install -y openjdk-17-jre-headless
WORKDIR /App

COPY --from=build-env /App/out ./DiscordBot/

ENTRYPOINT ["dotnet", "DiscordBot/PnPBot.dll"]
