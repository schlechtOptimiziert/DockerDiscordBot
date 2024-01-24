using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using PnPBot.Sevices;
using PnPBot.Commands;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

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

        await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN")); //To set a permanant SystemVariable =>  setx DISCORD_BOT_TOKEN ""
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
}