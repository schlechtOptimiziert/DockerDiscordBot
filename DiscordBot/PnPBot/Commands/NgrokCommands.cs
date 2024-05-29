using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using PnPBot.Sevices;

namespace PnPBot.Commands;

public class NgrokCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly NgrokService ngrokService;

    public NgrokCommands(NgrokService ngrokService)
    {
        this.ngrokService = ngrokService;
    }

    [SlashCommand("get-tunnels", "Gets the list of running tunnels.")]
    public async Task GetTunnelsAsync()
    {
        var tunnels = await ngrokService.GetTunnelsAsync().ConfigureAwait(false);

        var stringBuilder = new StringBuilder();
        foreach (var tunnel in tunnels)
            stringBuilder.AppendLine($"{tunnel.name} => {tunnel.public_url}");
        var description = stringBuilder.ToString();

        var embedBuiler = new EmbedBuilder()
            .WithDescription(string.IsNullOrWhiteSpace(description) ? "No tunnels currently active." : description)
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("get-tunnel", "Does shit.")]
    public async Task GetTunnelAsync(string name)
    {
        var tunnel = await ngrokService.GetTunnelAsync(name).ConfigureAwait(false);

        var embedBuiler = new EmbedBuilder()
            .WithDescription($"{tunnel.name} => {tunnel.public_url}")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("stop-tunnel", "Does shit.")]
    public async Task StopTunnelAsync(string name)
    {
        var tunnel = await ngrokService.StopTunnelAsync(name).ConfigureAwait(false);

        var embedBuiler = new EmbedBuilder()
            .WithDescription(tunnel ? "Done" : "Error")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("start-tunnel", "Does shit.")]
    public async Task StartTunnelAsync()
    {
        var tunnel = await ngrokService.StartTunnelAsync(new("McServerTunnel", "tcp", "192.168.0.10:25565")).ConfigureAwait(false);

        var embedBuiler = new EmbedBuilder()
            .WithDescription(tunnel ? "Done" : "Error")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }
}
