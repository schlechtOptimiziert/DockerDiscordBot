using Discord;
using Discord.Interactions;
using Docker.DotNet.Models;
using DiscordBot.Sevices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Sevices.Docker;

namespace DiscordBot.Commands;

public class ServerCommands : SlashCommandBase
{
    private readonly NgrokService ngrokService;
    private readonly DockerService dockerService;
    private readonly DockerServerService dockerServerService;

    public ServerCommands(NgrokService ngrokService, DockerService dockerService, DockerServerService dockerServerService)
    {
        this.ngrokService = ngrokService;
        this.dockerService = dockerService;
        this.dockerServerService = dockerServerService;
    }

    [SlashCommand("server-list", "Lists the servers.")]
    public async Task StartAsync()
    {
        await RespondAsync().ConfigureAwait(false);

        var stringBuilder = new StringBuilder();
        var serverNames = dockerServerService.GetServerNames();

        stringBuilder.AppendLine("Servers:");
        foreach (var serverName in serverNames)
        {
            var excistingContainers = await dockerService.GetConatinersAsync(new string[] {serverName}).ConfigureAwait(false);
            if (excistingContainers.Any())
            {
                var excistingContainer = excistingContainers.FirstOrDefault();
                var tunnel = await ngrokService.GetTunnelAsync(serverName).ConfigureAwait(false);

                stringBuilder.AppendLine($"({string.Join(", ", excistingContainer.Names)})");
                stringBuilder.AppendLine(excistingContainer.State);
                stringBuilder.AppendLine(tunnel.public_url);
                stringBuilder.AppendLine(string.Empty);
            }
            else
            {
                stringBuilder.AppendLine($"({serverName})");
                stringBuilder.AppendLine("offline");
                stringBuilder.AppendLine(string.Empty);
            }
        }

        await ModifyResponseAsync(stringBuilder.ToString(), Color.Green).ConfigureAwait(false);
    }

    [SlashCommand("server-start", "Starts a server.")]
    public async Task StartAsync(string serverName)
    {
        await RespondAsync().ConfigureAwait(false);

        var serverConfig = dockerServerService.GetServerContainer(serverName);
        if (serverConfig is null)
        {
            await ModifyResponseAsync($"Server with name '{serverName}' was not found.", Color.Red).ConfigureAwait(false);
            return;
        }

        var output = await dockerService.CreateContainerAsync(serverConfig).ConfigureAwait(false);
        if (!output)
        {
            await ModifyResponseAsync($"Server '{serverName}' could not be started.", Color.Red).ConfigureAwait(false);
            return;
        }

        output = await ngrokService.StartTunnelAsync(new(serverConfig.Name, "tcp", $"localhost:{serverConfig.NgrokPort}")).ConfigureAwait(false);
        if (!output)
        {
            await ModifyResponseAsync($"Tunnel for server '{serverName}' could not be created.", Color.Red).ConfigureAwait(false);
            return;
        }

        await ModifyResponseAsync("Done.", Color.Green).ConfigureAwait(false);
    }

    [SlashCommand("server-stop", "Stops a server.")]
    public async Task StopAsync(string serverName)
    {
        await RespondAsync().ConfigureAwait(false);

        var serverConfig = dockerServerService.GetServerContainer(serverName);
        if (serverConfig is null)
        {
            await ModifyResponseAsync($"Server with name '{serverName}' was not found.", Color.Red).ConfigureAwait(false);
            return;
        }

        await dockerService.RemoveContainerAsync(serverConfig).ConfigureAwait(false);
        await ngrokService.StopTunnelAsync(serverConfig.Name).ConfigureAwait(false);

        await ModifyResponseAsync("Done.", Color.Green).ConfigureAwait(false);
    }
}
