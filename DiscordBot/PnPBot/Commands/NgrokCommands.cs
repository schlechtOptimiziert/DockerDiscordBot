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

    [SlashCommand("tunnels", "Gets the list of running tunnels.")]
    public async Task GetTunnels()
    {
        var tunnels = await ngrokService.GetTunnelsAsync().ConfigureAwait(false);

        var stringBuilder = new StringBuilder();
        foreach (var tunnel in tunnels)
            stringBuilder.AppendLine($"{tunnel.Name} => {tunnel.Public_url}");

        var embedBuiler = new EmbedBuilder()
            .WithTitle("Tunnels:")
            .WithDescription(stringBuilder.ToString())
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await RespondAsync(embed: embedBuiler.Build());
    }
}
