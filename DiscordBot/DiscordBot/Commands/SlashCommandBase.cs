using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiscordBot.Commands;

public class SlashCommandBase : InteractionModuleBase<SocketInteractionContext>
{
    public Task RespondAsync()
        => RespondAsync(embed: GetDefaultEmbedBuilder().Build());

    public Task ModifyResponseAsync(string message, Color color)
    {
        var embedBuilder = GetDefaultEmbedBuilder().WithColor(color).WithDescription(message);
        return ModifyOriginalResponseAsync(message => message.Embed = embedBuilder.Build());
    }

    private EmbedBuilder GetDefaultEmbedBuilder()
        => new EmbedBuilder()
            .WithDescription("On it.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
}
