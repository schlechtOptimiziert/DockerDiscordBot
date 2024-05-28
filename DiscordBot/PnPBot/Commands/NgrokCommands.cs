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
            stringBuilder.AppendLine($"{tunnel.Name} => {tunnel.Public_url}");

        var embedBuiler = new EmbedBuilder()
            .WithDescription(stringBuilder.ToString())
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("get-tunnel", "")]
    public async Task GetTunnelAsync(string name)
    {
        var tunnel = await ngrokService.GetTunnelAsync(name).ConfigureAwait(false);

        var embedBuiler = new EmbedBuilder()
            .WithDescription($"{tunnel.Name} => {tunnel.Public_url}")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("stop-tunnel", "")]
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
