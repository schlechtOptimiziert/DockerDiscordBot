using System;
using System.Threading.Tasks;
using DiscordBot.Exceptions;
using RconSharp;

namespace DiscordBot.Sevices;

public class RconService
{
    private readonly string rconIp;
    private readonly string rconPassword;

    public RconService()
    {
        rconIp = Environment.GetEnvironmentVariable("RCON_IP") ?? throw new ConfigurationException("Environment variable 'RCON_IP' has not been set.");
        rconPassword = Environment.GetEnvironmentVariable("RCON_PASSWORD") ?? throw new ConfigurationException("Environment variable 'RCON_PASSWORD' has not been set.");
    }

    public async Task<string> ExecuteRconCommand(string command)
    {
        var rconClient = RconClient.Create(rconIp, 25575);
        await rconClient.ConnectAsync().ConfigureAwait(false);
        if (!await rconClient.AuthenticateAsync(rconPassword).ConfigureAwait(false))
            return "Rcon authentication failed. Could be cause of a incorrect rcon password. This is a configuration failiure. This has nothing to do with the discord bot or its user. :)";

        var result = await rconClient.ExecuteCommandAsync(command).ConfigureAwait(false);
        rconClient.Disconnect();

        return result;
    }
}
