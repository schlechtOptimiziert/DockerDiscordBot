using System;
using System.Threading.Tasks;
using DiscordBot.Exceptions;
using DiscordBot.Sevices.Docker;
using RconSharp;

namespace DiscordBot.Sevices;

public class RconService
{
    public async Task<string> ExecuteRconCommand(ServerConfig dockerContainer, string command)
    {
        var rconPassword = dockerContainer.EnvironmentVariables["RCON_PASSWORD"];

        var rconClient = RconClient.Create("localhost", 25575);
        await rconClient.ConnectAsync().ConfigureAwait(false);
        if (!await rconClient.AuthenticateAsync(rconPassword).ConfigureAwait(false))
            return "Rcon authentication failed. Could be cause of a incorrect rcon password. This is a configuration failiure. This has nothing to do with the discord bot or its user. :)";

        var result = await rconClient.ExecuteCommandAsync(command).ConfigureAwait(false);
        rconClient.Disconnect();

        return result;
    }
}
