using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Sevices;

namespace DiscordBot.Commands;

public class NgrokCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly NgrokService ngrokService;

    public NgrokCommands(NgrokService ngrokService)
    {
        this.ngrokService = ngrokService;
    }

    [RequireOwner]
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

    [RequireOwner]
    [SlashCommand("stop-tunnel", "Stops a tunnel.")]
    public async Task StopTunnelAsync(string name)
    {
        var tunnel = await ngrokService.StopTunnelAsync(name).ConfigureAwait(false);

        var embedBuiler = new EmbedBuilder()
            .WithDescription(tunnel ? "Done" : "Error")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }
}
