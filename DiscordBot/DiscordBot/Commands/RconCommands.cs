using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Sevices;

namespace DiscordBot.Commands;

public sealed class RconCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly RconService rconService;

    public RconCommands(RconService rconService)
    {
        this.rconService = rconService;
    }

    [SlashCommand("mc-whitelist-add", "Whitelists a user.")]
    public Task WhitelistAddCommand(string username)
        => CommandInteractionAsync($"whitelist add {username}");

    [SlashCommand("mc-whitelist-list", "Get whitelist list.")]
    public Task WhitelistListCommand()
        => CommandInteractionAsync($"whitelist list");

    [SlashCommand("mc-whitelist-remove", "Remove a user from the whitelist.")]
    public Task WhitelistRemoveCommand(string username)
        => CommandInteractionAsync($"whitelist remove {username}");

    private async Task CommandInteractionAsync(string command)
    {
        var embedBuiler = new EmbedBuilder()
            .WithDescription("On it.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());

        var output = await rconService.ExecuteRconCommand(command).ConfigureAwait(false);

        embedBuiler = new EmbedBuilder()
            .WithDescription(output)
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await FollowupAsync(embed: embedBuiler.Build()).ConfigureAwait(false);
    }
}
