using Discord;
using Discord.Interactions;
using DiscordBot.Sevices;
using DiscordBot.Sevices.Docker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands;

public class TestCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DockerBlueprintService dockerBlueprintService;

    public TestCommands(DockerBlueprintService dockerBlueprintService)
    {
        this.dockerBlueprintService = dockerBlueprintService;
    }

    [SlashCommand("test", "Test halt")]
    public async Task TestCommand()
    {
        var t = dockerBlueprintService.GetBlueprintNames();

        var builder = new StringBuilder();
        foreach (var name in t)
            builder.AppendLine(name);

        var embedBuiler = new EmbedBuilder()
            .WithDescription(builder.ToString())
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());
    }
}
