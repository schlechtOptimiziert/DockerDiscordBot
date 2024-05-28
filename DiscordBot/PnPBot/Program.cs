using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PnPBot.Exceptions;
using PnPBot.Sevices;
using PnPBot.Sevices;

namespace PnPBot;

public class Program
{
    public static async Task Main()
    {
        var _serviceProvider = new ServiceCollection()
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton<InteractionHandler>()

            .AddSingleton<NgrokService>()
            .AddSingleton<RconService>()
            .AddSingleton<DockerService>()

            .BuildServiceProvider();

        var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        await _serviceProvider.GetRequiredService<InteractionHandler>().InitializeAsync();

        var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? throw new ConfigurationException("Environment variable 'DISCORD_BOT_TOKEN' has not been set.");
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}