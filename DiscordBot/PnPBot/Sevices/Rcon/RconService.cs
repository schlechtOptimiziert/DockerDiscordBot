using System;
using System.Threading.Tasks;
using PnPBot.Exceptions;
using RconSharp;

namespace PnPBot.Sevices.Rcon;

public class RconService
{
    private readonly RconClient rconClient;
    private readonly string rconPassword;

    public RconService()
    {
        var rconIp = Environment.GetEnvironmentVariable("RCON_IP") ?? throw new ConfigurationException("Environment variable 'RCON_IP' has not been set.");
        rconPassword = Environment.GetEnvironmentVariable("RCON_PASSWORD") ?? throw new ConfigurationException("Environment variable 'RCON_PASSWORD' has not been set.");
        rconClient = RconClient.Create(rconIp, 25575);
    }

    public async Task<string> AddToWhitelistAsync(string name)
    {
        await rconClient.ConnectAsync().ConfigureAwait(false);
        if (!await rconClient.AuthenticateAsync(rconPassword).ConfigureAwait(false))
            return "Rcon authentication failed. Could be cause of a incorrect rcon password. This is a configuration failiure. This has nothing to do with the discord bot or its user. :)";

        return await rconClient.ExecuteCommandAsync($"whitelist add {name}").ConfigureAwait(false);
    }
}
