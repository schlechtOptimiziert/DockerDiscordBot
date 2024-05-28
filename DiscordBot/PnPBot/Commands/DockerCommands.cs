using Aspose.Cells;
using Discord;
using Discord.Interactions;
using Docker.DotNet.Models;
using PnPBot.Sevices;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PnPBot.Commands;

public class DockerCommands : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DockerService dockerService;

    public DockerCommands(DockerService dockerService)
    {
        this.dockerService = dockerService;
    }

    [SlashCommand("docker-list", "Gets a list of docker containers.")]
    public async Task DockerContainerList()
    {
        var embedBuiler = new EmbedBuilder()
            .WithDescription("On it.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());

        embedBuiler = new EmbedBuilder()
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        var output = await dockerService.GetConatinersAsync().ConfigureAwait(false);
        embedBuiler.WithDescription(ConatinerToCsv(output));

        await ModifyOriginalResponseAsync(message => message.Embed = embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("docker-pause", "pauses a container.")]
    public async Task DockerContainerPause(string Id)
    {
        var embedBuiler = new EmbedBuilder()
            .WithDescription("On it.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());


        await dockerService.PauseContainerAsync(Id).ConfigureAwait(false);

        embedBuiler = new EmbedBuilder()
            .WithDescription("Done.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await ModifyOriginalResponseAsync(message => message.Embed = embedBuiler.Build()).ConfigureAwait(false);
    }

    [SlashCommand("docker-unpause", "unpauses a container.")]
    public async Task DockerContainerUnpause(string Id)
    {
        var embedBuiler = new EmbedBuilder()
            .WithDescription("On it.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();
        await RespondAsync(embed: embedBuiler.Build());


        await dockerService.UnpauseContainerAsync(Id).ConfigureAwait(false);

        embedBuiler = new EmbedBuilder()
            .WithDescription("Done.")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp();

        await ModifyOriginalResponseAsync(message => message.Embed = embedBuiler.Build()).ConfigureAwait(false);
    }

    public string ConatinerToCsv(IEnumerable<ContainerListResponse> containers)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Image name,Id,Status");
        foreach (var item in containers)
            csv.AppendLine($"{item.Image},{item.ID},{item.State}");

        return csv.ToString();
    }
}
