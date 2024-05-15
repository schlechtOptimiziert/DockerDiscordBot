using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PnPBot.Commands;
using PnPBot.Exceptions;
using PnPBot.Sevices;
using PnPBot.Sevices.Rcon;

namespace PnPBot;

public class Program
{
    private readonly IServiceProvider _serviceProvider = CreateProvider();

    static void Main()
        => new Program().RunAsync().GetAwaiter().GetResult();

    static IServiceProvider CreateProvider()
    {
        var collection = new ServiceCollection();

        collection.AddSingleton<DiscordSocketClient>();
        collection.AddSingleton<CommandService>();

        collection.AddSingleton<Logger>();
        collection.AddSingleton<NgrokService>();
        collection.AddSingleton<RconService>();

        collection.AddSingleton<CommandHandler>();
        collection.AddSingleton<TextCommands>();

        return collection.BuildServiceProvider();
    }

    async Task RunAsync()
    {
        var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

        client.Log += _serviceProvider.GetRequiredService<Logger>().Log;
        client.Ready += _serviceProvider.GetRequiredService<CommandHandler>().Client_Ready;
        client.SlashCommandExecuted += _serviceProvider.GetRequiredService<CommandHandler>().SlashCommandHandler;

        var token = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? throw new ConfigurationException("Environment variable 'DISCORD_BOT_TOKEN' has not been set.");
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}