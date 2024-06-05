using Discord;
using Discord.Interactions;
using Docker.DotNet.Models;
using DiscordBot.Sevices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands;

public class MinecraftCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly string tunnelName = "McServerTunnel";
    private readonly string imageName = "itzg/minecraft-server";

    private readonly NgrokService ngrokService;
    private readonly DockerService dockerService;

    public MinecraftCommands(NgrokService ngrokService, DockerService dockerService)
    {
        this.ngrokService = ngrokService;
        this.dockerService = dockerService;
    }

    [SlashCommand("mc-status", "Gets the status of the minecraft server.")]
    public async Task GetStatusAsync()
    {
        var embedBuiler = new EmbedBuilder()
            .WithDescription("Fetching...")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());

        var tunnel = await GetTunnelAsync().ConfigureAwait(false);
        var container = await GetContainerAsync().ConfigureAwait(false);
        
        var stringBuilder = new StringBuilder();
        if (tunnel is not null)
        {
            stringBuilder.AppendLine("tunnel is running");
            stringBuilder.AppendLine($"{tunnel.name} => {tunnel.public_url}");
        }
        else
        {
            stringBuilder.AppendLine("tunnel is offline");
        }

        if (container is not null)
            stringBuilder.AppendLine($"Server is {container.State}");
        else
            stringBuilder.AppendLine($"Server is not running");

        embedBuiler.WithDescription(stringBuilder.ToString());
        await ModifyOriginalResponseAsync(message => message.Embed = embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("mc-start", "Starts the minecraft server.")]
    public async Task StartAsync()
    {
        var embedBuiler = new EmbedBuilder()
            .WithDescription("On it.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());

        await StartTunnelAsync().ConfigureAwait(false);
        await CreateContainerAsync().ConfigureAwait(false);


        embedBuiler.WithDescription("Done.");
        await ModifyOriginalResponseAsync(message => message.Embed = embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("mc-stop", "Stops the minecraft server.")]
    public async Task StopAsync()
    {
        var embedBuiler = new EmbedBuilder()
            .WithDescription("On it.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());

        await StopTunnelAsync().ConfigureAwait(false);
        await RemoveContainerAsync().ConfigureAwait(false);

        embedBuiler.WithDescription("Done.");
        await ModifyOriginalResponseAsync(message => message.Embed = embedBuiler.Build()).ConfigureAwait(false);
    }

    private Task<bool> StartTunnelAsync()
        => ngrokService.StartTunnelAsync(new(tunnelName, "tcp", "localhost:25565"));

    private Task<bool> StopTunnelAsync()
        => ngrokService.StopTunnelAsync(tunnelName);

    private Task<NgrokTunnelResponeBody> GetTunnelAsync()
        => ngrokService.GetTunnelAsync(tunnelName);

    private async Task<ContainerListResponse> GetContainerAsync()
        => (await dockerService.GetConatinersAsync(imageName).ConfigureAwait(false)).FirstOrDefault();

    private async Task<bool> RemoveContainerAsync()
    {
        var container = await GetContainerAsync().ConfigureAwait(false);
        if (container is null)
            return false;

        return await dockerService.RemoveContainerAsync(container.ID).ConfigureAwait(false);
    }

    private Task<bool> CreateContainerAsync()
        => dockerService.CreateContainerAsync(new());
}
