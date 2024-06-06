using Discord;
using Discord.Interactions;
using Docker.DotNet.Models;
using DiscordBot.Sevices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Sevices.Docker;

namespace DiscordBot.Commands;

public class MinecraftCommands : SlashCommandBase
{
    private readonly NgrokService ngrokService;
    private readonly DockerService dockerService;
    private readonly DockerBlueprintService dockerBlueprintService;

    public MinecraftCommands(NgrokService ngrokService, DockerService dockerService, DockerBlueprintService dockerBlueprintService)
    {
        this.ngrokService = ngrokService;
        this.dockerService = dockerService;
        this.dockerBlueprintService = dockerBlueprintService;
    }

    //[SlashCommand("mc-status", "Gets the status of the minecraft server.")]
    //public async Task GetStatusAsync()
    //{ }

    [SlashCommand("mc-start", "Starts the minecraft server.")]
    public async Task StartAsync(string name)
    {
        await RespondAsync().ConfigureAwait(false);

        var container = dockerBlueprintService.GetBlueprint(name);
        if (container is null)
        {
            await ModifyResponseAsync($"Containerblueprint with name '{name}' was not found.", Color.Red).ConfigureAwait(false);
            return;
        }

        var output = await dockerService.CreateContainerAsync(container).ConfigureAwait(false);
        if (!output)
        {
            await ModifyResponseAsync($"Container could not be created with blueprint '{name}'.", Color.Red).ConfigureAwait(false);
            return;
        }

        output = await ngrokService.StartTunnelAsync(new(container.Name, "tcp", "localhost:25565")).ConfigureAwait(false);
        if (!output)
        {
            await ModifyResponseAsync($"Tunnel could not be created '{name}'.", Color.Red).ConfigureAwait(false);
            return;
        }

        await ModifyResponseAsync("Done.", Color.Green).ConfigureAwait(false);
    }

    [SlashCommand("mc-stop", "Stops the minecraft server.")]
    public async Task StopAsync(string name)
    {
        await RespondAsync().ConfigureAwait(false);

        var container = dockerBlueprintService.GetBlueprint(name);
        if (container is null)
        {
            await ModifyResponseAsync($"Containerblueprint with name '{name}' was not found.", Color.Red).ConfigureAwait(false);
            return;
        }

        await dockerService.RemoveContainerAsync(container).ConfigureAwait(false);
        await ngrokService.StopTunnelAsync(container.Name).ConfigureAwait(false);

        await ModifyResponseAsync("Done.", Color.Green).ConfigureAwait(false);
    }
}
