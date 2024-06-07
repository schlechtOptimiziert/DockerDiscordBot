using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DiscordBot.Sevices;
using DiscordBot.Sevices.Docker;

namespace DiscordBot.Commands;

public sealed class RconCommands : SlashCommandBase
{
    private readonly RconService rconService;
    private readonly DockerServerService dockerServerService;

    public RconCommands(RconService rconService, DockerServerService dockerServerService)
    {
        this.rconService = rconService;
        this.dockerServerService = dockerServerService;
    }

    [SlashCommand("mc-whitelist-add", "Whitelists a user.")]
    public Task WhitelistAddCommand(string serverName, string username)
        => CommandInteractionAsync(serverName, $"whitelist add {username}");

    [SlashCommand("mc-whitelist-list", "Get whitelist list.")]
    public Task WhitelistListCommand(string serverName)
        => CommandInteractionAsync(serverName, $"whitelist list");

    [SlashCommand("mc-whitelist-remove", "Remove a user from the whitelist.")]
    public Task WhitelistRemoveCommand(string serverName, string username)
        => CommandInteractionAsync(serverName, $"whitelist remove {username}");

    private async Task CommandInteractionAsync(string serverName, string command)
    {
        await RespondAsync().ConfigureAwait(false);

        var serverConfig = dockerServerService.GetServerContainer(serverName);
        if (serverConfig is null)
        {
            await ModifyResponseAsync($"Server with name '{serverName}' was not found.", Color.Red).ConfigureAwait(false);
            return;
        }

        var output = await rconService.ExecuteRconCommand(serverConfig, command).ConfigureAwait(false);

        await ModifyResponseAsync(output, Color.Green).ConfigureAwait(false);
    }
}
