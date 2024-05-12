using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PnPBot.Sevices;

namespace PnPBot.Commands;

public sealed class TextCommands
{
    private readonly NgrokService ngrokService;

    public TextCommands(NgrokService ngrokService)
    {
        this.ngrokService = ngrokService;
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
}
