# Setup
## Installs
- [Docker](https://www.docker.com/products/docker-desktop/)
## Set important tokens
1. open the (docker-compose.yml)[https://github.com/schlechtOptimiziert/DockerDiscordBot/blob/main/docker-compose.yml]
2. Set NGROK_AUTHTOKEN (this is important to make the server accessible over the internet)
3. Set DISCORD_BOT_TOKEN (this is important if you want to use your discord bot to get the server ip and whitelist people)

# Run
1. Open a terminal in the folder with the docker compose
2. Paste "docker composer up -d" into the terminal + execute it
3. Wait until the container are up and enjoy ;)