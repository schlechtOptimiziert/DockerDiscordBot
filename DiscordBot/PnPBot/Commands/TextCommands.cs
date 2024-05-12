using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PnPBot.Sevices;
using PnPBot.Sevices.Rcon;

namespace PnPBot.Commands;

public sealed class TextCommands
{
    private readonly NgrokService ngrokService;
    private readonly RconService rconService;

    public TextCommands(NgrokService ngrokService, RconService rconService)
    {
        this.ngrokService = ngrokService;
        this.rconService = rconService;
    }

    public async Task GetTunnelsCommand(SocketSlashCommand command)
    {
        var tunnels = await ngrokService.GetTunnelsAsync().ConfigureAwait(false);

        var stringBuilder = new StringBuilder();
        foreach (var tunnel in tunnels)
        {
            stringBuilder.AppendLine($"{tunnel.Name} => {tunnel.Public_url}");
        }

        var embedBuiler = new EmbedBuilder()
            .WithTitle("Tunnels:")
            .WithDescription(stringBuilder.ToString())
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuiler.Build());
    }

    public async Task WhitelistAddCommand(SocketSlashCommand command)
    {
        var output = await rconService.AddToWhitelistAsync(command.Data.Options.First().Value.ToString()).ConfigureAwait(false);

        var embedBuiler = new EmbedBuilder()
            .WithTitle("WhitelistAdd:")
            .WithDescription(output)
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuiler.Build());
    }
}
