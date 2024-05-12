using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace PnPBot.Commands;

public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly TextCommands _commands;

    public CommandHandler(DiscordSocketClient client, TextCommands commands)
    {
        _client = client;
        _commands = commands;
    }

    public async Task Client_Ready()
    {
        var guildCommands = new List<SlashCommandBuilder>()
        {
            new SlashCommandBuilder()
                .WithName("tunnels").WithDescription("Gets the list of running tunnels.")
        };

        try
        {
            foreach (var command in guildCommands)
                await _client.Rest.CreateGlobalCommand(command.Build());
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }

    public async Task SlashCommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "tunnels":
                await _commands.GetTunnelsCommand(command);
                break;
        }
    }
}
